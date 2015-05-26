using Caliburn.Micro;
using Komissarov.Nsu.OracleClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komissarov.Nsu.OracleClient.ViewModels.Tabs
{
	class TableEditorViewModel : PropertyChangedBase
	{
		private IAccessProvider _provider;
		private string _tableName;

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

		public TableEditorViewModel( IAccessProvider provider, string tableName )
		{
			_provider = provider;

			//stub
			Columns = new BindableCollection<Column>( );
			for ( int i = 0; i < 20; ++i )
				Columns.Add( new Column( )
				{
					Name = i.ToString( ),
					Type = i.ToString( ),
					Nullable = i % 2 == 0
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
