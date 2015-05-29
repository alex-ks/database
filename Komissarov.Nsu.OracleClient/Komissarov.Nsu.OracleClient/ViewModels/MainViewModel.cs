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

		public event ConnectHandler ConnectEvent;

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
				if ( value && ConnectEvent != null )
					ConnectEvent( );
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

		private TableBrowserViewModel _browser;
		public TableBrowserViewModel Browser
		{
			set
			{
				if ( _browser == value )
					return;
				_browser = value;
				NotifyOfPropertyChange( ( ) => Browser );
			}
			get
			{
				return _browser;
			}
		}

		public MainViewModel( IWindowManager manager )
		{
			_manager = manager;
			_accessor = null;
			Connected = false;

			QueryTab = new QueryViewModel( _manager, this );
			QueryTab.RequireUpdate += UpdateHandler;
			Browser = new TableBrowserViewModel( _manager, this );
			Browser.RequireUpdate += UpdateHandler;
		}

		public void LogOut( )
		{
			_accessor.Dispose( );
			_accessor = null;
			Connected = false;
		}

		public void LogIn( )
		{
			AuthorizationViewModel authorization = new AuthorizationViewModel( )
			{
				DisplayName = "Authorization"
			};
			_manager.ShowDialog( authorization );

			_accessor = authorization.Accessor;

			if ( _accessor != null )
				Connected = true;
		}

		public void MakeReport( )
		{
			MessageBox.Show( "Oracle client v1.0 build 33" + Environment.NewLine + "Developed by Alexander Inc." + Environment.NewLine + "All rights reserved", "About", MessageBoxButton.OK, MessageBoxImage.Asterisk );
		}

		public void ReportError( string message )
		{
			MessageBox.Show( message, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
		}

		public void UpdateHandler( )
		{
			Browser = new TableBrowserViewModel( _manager, this );
			Accessor.ResetAllLists( );
		}

		public OracleAccessor Accessor
		{
			get
			{
				return _accessor;
			}
		}
	}
}
