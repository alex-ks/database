using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Komissarov.Nsu.OracleClient.Accessor;
using Komissarov.Nsu.OracleClient.ViewModels.Tabs;

namespace Komissarov.Nsu.OracleClient.ViewModels
{
	class MainViewModel : PropertyChangedBase, IAccessProvider
	{
		private IWindowManager _manager;
		private OracleAccessor _accessor;

		private string _stub;
		public string Stub
		{
			get
			{
				return _stub;
			}
			set
			{
				if ( value == _stub )
					return;
				_stub = value;
				NotifyOfPropertyChange( ( ) => Stub );
			}
		}

		private bool _connected;

		public bool Connected
		{
			get
			{
				return _connected;
			}
			set
			{
				if ( value == _connected )
					return;
				_connected = value;
				NotifyOfPropertyChange( ( ) => Connected );
				NotifyOfPropertyChange( ( ) => Disconnected );
			}
		}

		public bool Disconnected
		{
			get
			{
				return !Connected;
			}
			set
			{
				Connected = !value;
			}
		}

		//Tabs properties

		public QueryViewModel QueryTab
		{
			set;
			get;
		}

		public MainViewModel( IWindowManager manager )
		{
			_manager = manager;
			_accessor = null;
			Connected = false;
			QueryTab = new QueryViewModel( _manager, this );
			QueryTab.Disconnected += DisconnectedHandler;
		}

		public void LogOut( )
		{
			_accessor.Dispose( );
			_accessor = null;
			Connected = false;
		}

		public void LogIn( )
		{
			AuthorizationViewModel authorization = new AuthorizationViewModel( );
			_manager.ShowDialog( authorization );

			_accessor = authorization.Accessor;

			if ( _accessor != null )
			{
				Connected = true;

				foreach ( var name in _accessor.GetAvaliableTables( ) )
					Stub += name + '\n';
			}
		}

		public void MakeReport( )
		{
			MessageBox.Show( "WAAAAAAAGH!", "Report" );
		}

		public void DisconnectedHandler( )
		{
			//todo: handle disconnect
		}

		public OracleAccessor Accessor
		{
			get { return _accessor; }
		}
	}
}
