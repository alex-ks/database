using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Xml;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Komissarov.Nsu.OracleClient.Accessor
{
	public class OracleAccessor : IDisposable
	{
		private OracleConnection _connection;
		private List<string> _avaliableTables;
		private List<string> _avaliableTypes;
		private string _userId;
		private string _tablespace;
		private StringLoader _loader;
		//private string _infoGettingQuery;

		public OracleAccessor( string dataSource, string userId, string password, string tablespace = null )
		{
			_loader = new StringLoader( );

			_avaliableTables = null;
			_avaliableTypes = null;
			_userId = userId;
			_tablespace = tablespace;

			var builder = new OracleConnectionStringBuilder( );
			builder.DataSource = dataSource;
			builder.Password = password;
			builder.UserID = userId;
			builder.ValidateConnection = true;

			_connection = new OracleConnection( builder.ToString( ) );
			_connection.Open( );
		}

		public OracleDataReader ExecuteQuery( string query )
		{
			using ( var command = _connection.CreateCommand( ) )
			{
				command.CommandText = query;
				return command.ExecuteReader( );
			}
		}

		public List<string> GetAvaliableTables( )
		{
			if ( _avaliableTables != null )
				return _avaliableTables;

			_avaliableTables = new List<string>( );

			using ( var command = _connection.CreateCommand( ) )
			{
				command.CommandText = "select * from ALL_TABLES";

				using ( var dataReader = command.ExecuteReader( ) )
				{
					var names = from DbDataRecord row in dataReader
								where !row[row.GetOrdinal( "owner" )].ToString( ).Equals( "SYS" )
								&& ( _tablespace == null || row[row.GetOrdinal( "tablespace_name" )].ToString( ).Equals( _tablespace ) )
								orderby row[row.GetOrdinal( "table_name" )]
								select row[row.GetOrdinal( "table_name" )].ToString( );

					_avaliableTables = names.ToList( );
				}
			}
			return _avaliableTables;
		}

		public List<string> GetAvaliableTypes( )
		{
			if ( _avaliableTypes != null )
				return _avaliableTypes;

			_avaliableTypes = new List<string>( );

			using ( var command = _connection.CreateCommand( ) )
			{
				command.CommandText = _loader.TypeGettingQuery;

				using ( var dataReader = command.ExecuteReader( ) )
				{
					var names = from DbDataRecord row in dataReader
								orderby row[row.GetOrdinal( "type_name" )]
								select row[row.GetOrdinal( "type_name" )].ToString( );

					_avaliableTypes = names.ToList( );
				}
			}
			return _avaliableTypes;
		}

		public OracleDataReader GetTableInfo( string tableName )
		{
			using ( var command = _connection.CreateCommand( ) )
			{
				command.CommandText = string.Format( _loader.InfoGettingQuery, tableName );
				return command.ExecuteReader( );
			}
		}

		public DataTable GetTableContent( string tableName )
		{
			using ( OracleCommand command = _connection.CreateCommand( ) )
			{
				command.CommandText = string.Format( "select * from {0}", tableName );
				using ( OracleDataAdapter adapter = new OracleDataAdapter( command ) )
				{
					DataTable table = new DataTable( );
					adapter.Fill( table );
					return table;
				}
			}
		}

		public void UpdateTableContent( string tableName, DataTable table )
		{
			using ( OracleCommand command = _connection.CreateCommand( ) )
			{
				command.CommandText = string.Format( "select * from {0}", tableName );
				using ( OracleDataAdapter adapter = new OracleDataAdapter( command ) )
				{
					OracleCommandBuilder builder = new OracleCommandBuilder( adapter );
					adapter.UpdateCommand = builder.GetUpdateCommand( );
					adapter.Update( table );
				}
			}
		}

		public void ResetAllLists( )
		{
			_avaliableTables = null;
			_avaliableTypes = null;
		}

		public void Dispose( )
		{
			_connection.Dispose( );
		}
	}
}
