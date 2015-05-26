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

namespace Komissarov.Nsu.OracleClient.ViewModels.Tabs
{
	class TableEditorViewModel : PropertyChangedBase
	{
		private IAccessProvider _provider;
		private string _tableName;

		public bool Created
		{
			private set;
			get;
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

		public List<string> AvaliableTables
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
			_provider = provider;
			if ( tableName == null )
				Created = true;
			else
				Created = false;

			//stub
			Columns = new BindableCollection<Column>( );
			for ( int i = 0; i < 20; ++i )
				Columns.Add( new Column( )
				{
					Name = i.ToString( ),
					Type = i.ToString( ),
					ForeignKey = i % 2 == 0
				} );
		}

		public void CommitChanges( )
		{
			
		}

		public void RevertChanges( )
		{
		
		}

		public void AddNew( )
		{
		
		}
	}
}
