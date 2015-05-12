using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Oracle.ManagedDataAccess.Client;

namespace Komissarov.Nsu.OracleClient.Accessor
{
	public class OracleAccessor : IDisposable
	{
		private OracleConnection _connection;
		private List<string> _avaliableTables;
		private string _userId;
		private string _tablespace;

		public OracleAccessor( string dataSource, string userId, string password, string tablespace = null )
		{
			_avaliableTables = null;
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

		public void ExecuteQuery( string query )
		{
			//todo: implement query execution

			//using ( var command = _connection.CreateCommand( ) )
			//{
			//	command.CommandText = query;

			//}
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
								where row[row.GetOrdinal( "owner" )].ToString( ).Equals( _userId )
								&& ( _tablespace == null || row[row.GetOrdinal( "tablespace_name" )].ToString( ).Equals( _tablespace ) )
								orderby row[row.GetOrdinal( "table_name" )]
								select row[row.GetOrdinal( "table_name" )].ToString( );

					_avaliableTables = names.ToList( );
				}
			}
			return _avaliableTables;
		}

		public void Dispose( )
		{
			_connection.Dispose( );
		}
	}
}
