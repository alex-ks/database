using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace Komissarov.Nsu.OracleClient.ViewModels
{
	class AuthorizationViewModel : PropertyChangedBase
	{
		private string _userName, _password, _tablespace;
		private IWindowManager _manager;

		public string UserName
		{
			get { return _userName; }
			set
			{
				if ( value == _userName )
					return;
				_userName = value;
				NotifyOfPropertyChange( ( ) => UserName );
			}
		}

		public AuthorizationViewModel( IWindowManager winManager )
		{
			_manager = winManager;
		}

		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				if ( value == _password )
					return;
				_password = value;
				NotifyOfPropertyChange( ( ) => Password );
			}
		}

		public string Tablespace
		{
			get
			{
				return _tablespace;
			}
			set
			{
				if ( value == _tablespace )
					return;
				_tablespace = value;
				NotifyOfPropertyChange( ( ) => Tablespace );
			}
		}

		public void LogIn( )
		{
			MessageBox.Show( "Alpha version functional is limited" );
			//log in
			_manager.ShowWindow( new AuthorizationViewModel( _manager ) );
		}
	}
}
