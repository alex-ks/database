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

		public TableBrowserViewModel Browser
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
			Browser = new TableBrowserViewModel( _manager, this );
			Browser.Disconnected += DisconnectedHandler;
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
				Connected = true;
		}

		public void MakeReport( )
		{
			ReportViewModel report;
			if ( _accessor != null )
			{
				report = new ReportViewModel( _accessor.GetTableInfo( "ROGOLEV_GOODS_RETURNED" ) );
				_manager.ShowDialog( report );
			}
			else	
				MessageBox.Show( "WAAAAAAAGH!", "Report" );
		}

		public void ReportError( string message )
		{
			MessageBox.Show( message, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
		}

		public void DisconnectedHandler( )
		{
			//todo: handle disconnect
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
