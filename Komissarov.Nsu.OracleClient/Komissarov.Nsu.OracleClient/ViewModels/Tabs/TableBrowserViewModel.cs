using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Komissarov.Nsu.OracleClient.ViewModels.Tabs
{
	class TableBrowserViewModel : PropertyChangedBase, IConnected
	{
		private IAccessProvider _provider;
		private IWindowManager _manager;

		public IEnumerable<string> TableNames
		{
			get
			{
				return _provider.Accessor.GetAvaliableTables( ).Where( str => SearchCriteria == null || str.ToLower( ).Contains( SearchCriteria.ToLower( ) ) );
			}
		}

		public string SelectedItem
		{
			get;
			set;
		}

		private string _criteria;
		public string SearchCriteria
		{
			get
			{
				return _criteria;
			}

			set
			{
				if ( value == _criteria )
					return;

				_criteria = value;
				NotifyOfPropertyChange( ( ) => SearchCriteria );
				NotifyOfPropertyChange( ( ) => TableNames );
			}
		}

		public TableBrowserViewModel( IWindowManager manager, IAccessProvider provider )
		{
			_provider = provider;
			_manager = manager;
			provider.ConnectEvent += ConnectHander;
		}

		public void ConnectHander( )
		{
			NotifyOfPropertyChange( ( ) => TableNames );
		}

		public event DisconnectHandler Disconnected;
	}
}
