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
		private const int DefaultLength = 256;

		private static HashSet<string> TypesWithLength = new HashSet<string>( )
		{
			"CHAR", "VARCHAR", "VARCHAR2", "NCHAR", "NVARCHAR2"
		};

		private IAccessProvider _provider;
		private bool _created, _nameModified = false;

		public event EditHandle TableEdited;

		private string _tableName, _originalName;
		public string TableName
		{
			set
			{
				if ( _tableName == value )
					return;

				_tableName = value;

				if ( _created )
					return;

				NotifyOfPropertyChange( ( ) => TableName );

				if ( _originalName == null )
					_originalName = value;

				if ( _originalName == _tableName )
					_nameModified = false;
				else
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
							select new EditableColumn( )
							{
								Name = row["column_name"].ToString( ),
								Type = row["data_type"].ToString( ),
								Nullable = row["nullable"].ToString( ).Equals( "Y" ),
								PrimaryKey = row["primary_key"].ToString( ).Equals( "P" ),
								ForeignKey = row["foreign_key"].ToString( ).Equals( "R" ),
								FKName = row["fk_symbol"].ToString( ),
								SourceTable = row["source_table"].ToString( ),
								SourceColumn = row["source_column"].ToString( )
							};

				Columns = new ObservableCollection<Column>( names );
			}
			NotifyOfPropertyChange( ( ) => Columns );
		}

		private List<string> AlterTable( )
		{
			StringBuilder builder = new StringBuilder( );
			List<Column> primaryList = new List<Column>( ), foreignAddList = new List<Column>( );
			List<EditableColumn> foreignChangeList = new List<EditableColumn>( );
			List<string> result = new List<string>( );
			int oldPKCount = 0, changedPKCount = 0;

			foreach ( Column column in Columns )
			{
				if ( column.Created )
				{
					builder.Append( "ALTER TABLE " ).Append( _originalName ).Append( ' ' );
					builder.Append( "ADD COLUMN " ).Append( column.Name ).Append( ' ' ).Append( column.Type );

					if ( TypesWithLength.Contains( column.Type ) )
						builder.Append( '(' ).Append( DefaultLength ).Append( ')' );

					if ( !column.Nullable )
						builder.Append( " NOT NULL" );
					else
						builder.Append( " NULL" );

					if ( column.PrimaryKey )
					{
						primaryList.Add( column );
						++changedPKCount;
					}
						
					if ( column.ForeignKey )
						foreignAddList.Add( column );

					result.Add( builder.ToString( ) );
					builder.Clear( );
				}
				else
				{
					EditableColumn eColumn = column as EditableColumn;

					if ( eColumn.NameChanged )
					{
						builder.Append( "ALTER TABLE " ).Append( _originalName ).Append( ' ' );
						builder.Append( "RENAME COLUMN ").Append( eColumn.OldName ).Append( " TO " ).Append( eColumn.Name );
						result.Add( builder.ToString( ) );
						builder.Clear( );
					}
					if ( eColumn.TypeChanged || eColumn.NullableChanged )
					{
						builder.Append( "ALTER TABLE " ).Append( _originalName ).Append( ' ' );
						builder.Append( "MODIFY " ).Append( column.Name ).Append( ' ' ).Append( eColumn.Type );

						if ( TypesWithLength.Contains( eColumn.Type ) )
							builder.Append( '(' ).Append( DefaultLength ).Append( ')' );

						if ( eColumn.NullableChanged )
						{
							if ( !eColumn.Nullable )
								builder.Append( " NOT NULL" );
							else
								builder.Append( " NULL" );
						}
						
						result.Add( builder.ToString( ) );
						builder.Clear( );
					}
					if ( eColumn.PrimaryChanged )
						++changedPKCount;

					if ( eColumn.PrimaryKey )
						primaryList.Add( eColumn );

					if ( ( eColumn.PrimaryKey && !eColumn.PrimaryChanged )
						|| ( !eColumn.PrimaryKey && eColumn.PrimaryChanged ) )
						++oldPKCount;

					if ( eColumn.ForeignChanged )
						foreignChangeList.Add( eColumn );
				}
			}

			if ( changedPKCount != 0 )
			{
				if ( oldPKCount != 0 )
				{
					builder.Append( "ALTER TABLE " ).Append( _originalName ).Append( ' ' );
					builder.Append( "DROP PRIMARY KEY" );
					result.Add( builder.ToString( ) );
					builder.Clear( );
				}

				if ( primaryList.Count != 0 )
				{
					builder.Append( "ALTER TABLE " ).Append( _originalName ).Append( ' ' );
					builder.Append( "ADD PRIMARY KEY " ).Append( '(' );

					foreach ( var column in primaryList )
						builder.Append( column.Name ).Append( ',' );

					builder.Remove( builder.Length - 1, 1 );
					builder.Append( ')' );

					result.Add( builder.ToString( ) );
					builder.Clear( );
				}
			}

			if ( foreignAddList.Count != 0 )
			{
				foreach ( Column refColumn in foreignAddList )
				{
					builder.Append( "ALTER TABLE " ).Append( _originalName ).Append( ' ' );
					builder.Append( "ADD FOREIGN KEY (" ).Append( refColumn.Name ).Append( ')' )
						.Append( "REFERENCES " ).Append( refColumn.SourceTable ).Append( '(' )
						.Append( refColumn.SourceColumn ).Append( ')' );

					result.Add( builder.ToString( ) );
					builder.Clear( );
				}
			}
			if ( foreignChangeList.Count != 0 )
			{
				foreach ( EditableColumn refColumn in foreignChangeList )
				{
					if ( refColumn.OldForeignKey )
					{
						builder.Append( "ALTER TABLE " ).Append( _originalName ).Append( ' ' );
						builder.Append( "DROP CONSTRAINT " ).Append( refColumn.FKName );
						result.Add( builder.ToString( ) );
						builder.Clear( );
					}

					if ( refColumn.ForeignKey )
					{
						builder.Append( "ALTER TABLE " ).Append( _originalName ).Append( ' ' );
						builder.Append( "ADD FOREIGN KEY (" ).Append( refColumn.Name ).Append( ')' )
							.Append( "REFERENCES " ).Append( refColumn.SourceTable ).Append( '(' )
							.Append( refColumn.SourceColumn ).Append( ')' );

						result.Add( builder.ToString( ) );
						builder.Clear( );
					}
				}
			}

			if ( _nameModified )
			{
				builder.Append( "ALTER TABLE " ).Append( _originalName ).Append( ' ' )
					.Append( "RENAME TO " ).Append( _tableName );
				result.Add( builder.ToString( ) );
				builder.Clear( );
			}

			return result;
		}

		private List<string> CreateTable( )
		{
			StringBuilder builder = new StringBuilder( "CREATE TABLE " );
			builder.Append( _tableName ).Append( " ( " );

			List<Column> primaryList = new List<Column>( ),
				foreignList = new List<Column>( );

			if ( Columns.Count != 0 )
			{
				builder.Append( Columns[0].Name ).Append( ' ' )
					.Append( Columns[0].Type );

				if ( TypesWithLength.Contains( Columns[0].Type ) )
					builder.Append( "( " ).Append( DefaultLength ).Append( " )" );

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

					if ( TypesWithLength.Contains( Columns[i].Type ) )
						builder.Append( "( " ).Append( DefaultLength ).Append( " )" );
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

			return new List<string>( ) { builder.ToString( ) };
		}

		public void CommitChanges( )
		{
			List<string> queryList = _created ? CreateTable( ) : AlterTable( );
			try
			{
				foreach ( string query in queryList )
					_provider.Accessor.ExecuteQuery( query ).Dispose( );

				TableName = _tableName.ToUpper( );

				if ( TableEdited != null )
					TableEdited( );

				MessageBox.Show( "Table successfully " + ( _created ? "created" : "edited" ), "Report", MessageBoxButton.OK, MessageBoxImage.Information );

				_created = false;
				LoadTableData( );
			}
			catch ( Exception e )
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
