using Caliburn.Micro;
using Komissarov.Nsu.OracleClient.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Komissarov.Nsu.OracleClient.ViewModels.Tabs
{
	internal delegate void EditHandle( );

	class TableEditorViewModel : PropertyChangedBase
	{
		private IAccessProvider _provider;
		private bool _created, _nameModified = false;

		public event EditHandle TableEdited;

		private string _tableName;
		public string TableName
		{
			set
			{
				if ( _tableName == value )
					return;

				_tableName = value;
				_nameModified = true;
			}
			get
			{
				return _tableName;
			}
		}

		public ObservableCollection<Column> Columns
		{
			private set;
			get;
		}

		public List<string> AvaliableTypes
		{
			get
			{
				return _provider.Accessor.GetAvaliableTypes( );
			}
		}

		public Column SelectedColumn
		{
			set;
			get;
		}

		public IEnumerable<string> AvaliableTables
		{
			get
			{
				return _provider.Accessor.GetAvaliableTables( );
			}
		}

		public List<string> AvaliableColumnsToRef
		{
			get
			{
				if ( SelectedColumn.SourceTable == null )
					return null;

				try
				{
					using ( var dataReader = _provider.Accessor.GetTableInfo( SelectedColumn.SourceTable ) )
					{
						var names = from DbDataRecord row in dataReader
									where row[row.GetOrdinal( "primary_key" )].Equals( "P" )
									orderby row[row.GetOrdinal( "column_name" )]
									select row[row.GetOrdinal( "column_name" )].ToString( );

						return names.ToList( );
					}
				}
				catch ( OracleException )
				{
					return null;
				}

				//return null;
			}
		}

		public TableEditorViewModel( IAccessProvider provider, string tableName )
		{
			if ( tableName == null )
				_created = true;
			else
				_created = false;

			_provider = provider;
			TableName = tableName;

			LoadTableData( );
		}

		private void LoadTableData( )
		{
			using ( var dataReader = _provider.Accessor.GetTableInfo( TableName ) )
			{
				var names = from DbDataRecord row in dataReader
							select new Column( )
							{
								Created = false,
								Name = row[row.GetOrdinal( "column_name" )].ToString( ),
								Type = row[row.GetOrdinal( "data_type" )].ToString( ),
								Length = row[row.GetOrdinal( "data_length" )].ToString( ),
								Nullable = row[row.GetOrdinal( "nullable" )].ToString( ).Equals( "Y" ),
								PrimaryKey = row[row.GetOrdinal( "primary_key" )].ToString( ).Equals( "P" ),
								ForeignKey = row[row.GetOrdinal( "foreign_key" )].ToString( ).Equals( "R" ),
								SourceTable = row[row.GetOrdinal( "source_table" )].ToString( ),
								SourceColumn = row[row.GetOrdinal( "source_column" )].ToString( )
							};

				Columns = new ObservableCollection<Column>( names );
			}
			NotifyOfPropertyChange( ( ) => Columns );
		}

		private string AlterTable( )
		{
			StringBuilder builder = new StringBuilder( "ALTER TABLE " );
			//todo: make alter table query
			return builder.ToString( );
		}

		private string CreateTable( )
		{
			StringBuilder builder = new StringBuilder( "CREATE TABLE " );
			builder.Append( _tableName ).Append( " ( " );

			List<Column> primaryList = new List<Column>( ),
				foreignList = new List<Column>( );

			if ( Columns.Count != 0 )
			{
				builder.Append( Columns[0].Name ).Append( ' ' )
					.Append( Columns[0].Type );

				if ( Columns[0].Length != null )
					builder.Append( "( " ).Append( Columns[0].Length ).Append( " )" );
				
				builder.Append( ' ' );

				if ( !Columns[0].Nullable )
					builder.Append( "NOT NULL" );

				if ( Columns[0].PrimaryKey )
					primaryList.Add( Columns[0] );
				if ( Columns[0].ForeignKey )
					foreignList.Add( Columns[0] );

				for ( int i = 1; i < Columns.Count; ++i )
				{
					builder.Append( ", " ).Append( Columns[i].Name ).Append( ' ' )
						.Append( Columns[i].Type );

					if ( Columns[i].Length != null )
						builder.Append( "( " ).Append( Columns[i].Length ).Append( " )" );
					builder.Append( ' ' );

					if ( !Columns[i].Nullable )
						builder.Append( "NOT NULL" );

					if ( Columns[i].PrimaryKey )
						primaryList.Add( Columns[i] );
					if ( Columns[i].ForeignKey )
						foreignList.Add( Columns[i] );
				}
			}

			if ( primaryList.Count != 0 )
			{
				builder.Append( ", PRIMARY KEY ( " ).Append( primaryList[0].Name );
				for ( int i = 1; i < primaryList.Count; ++i )
					builder.Append( ", " ).Append( primaryList[i] );
				builder.Append( " )" );
			}

			if ( foreignList.Count != 0 )
			{
				foreach ( Column column in foreignList )
				{
					builder.Append( ", FOREIGN KEY ( " ).Append( column.Name ).Append( " ) " )
						.Append( "REFERENCES " ).Append( column.SourceTable ).Append( "( " )
						.Append( column.SourceColumn ).Append( " )" );
				}
			}

			builder.Append( " )" );

			return builder.ToString( );
		}

		public void CommitChanges( )
		{
			string query = _created ? CreateTable( ) : AlterTable( );
			try
			{
				_provider.Accessor.ExecuteQuery( query ).Dispose( );
				
				foreach ( Column column in Columns )
					column.Created = false;

				_created = false;

				if ( TableEdited != null )
					TableEdited( );

				MessageBox.Show( "Table successfully " + ( _created ? "created" : "edited" ), "Report", MessageBoxButton.OK, MessageBoxImage.Information );
			}
			catch ( OracleException e )
			{
				_provider.ReportError( e.Message );
			}
		}

		public void RevertChanges( )
		{
			LoadTableData( );
		}
	}
}
