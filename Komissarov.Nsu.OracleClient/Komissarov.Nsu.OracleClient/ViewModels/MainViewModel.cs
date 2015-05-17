using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Komissarov.Nsu.OracleClient.Accessor;

namespace Komissarov.Nsu.OracleClient.ViewModels
{
	class MainViewModel : PropertyChangedBase
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

		public MainViewModel( IWindowManager manager )
		{
			_manager = manager;
			_accessor = null;
			Connected = false;
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
	}
}
