using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Komissarov.Nsu.OracleClient.Accessor;
using Komissarov.Nsu.OracleClient.Views;

namespace Komissarov.Nsu.OracleClient.ViewModels
{
	class AuthorizationViewModel : Screen//, IViewAware
	{
		private string _dataSource, _userName, _tablespace;
		private IWindowManager _manager;

		public string DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				if ( value == _dataSource )
					return;
				_dataSource = value;
				NotifyOfPropertyChange( ( ) => DataSource );
			}
		}

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

		public AuthorizationViewModel( IWindowManager winManager )
		{
			_manager = winManager;
			DataSource = "10.4.0.119";
			UserName = "12201";
		}

		public void LogIn( )
		{
			var authorizationView = GetView( ) as AuthorizationView;
			if ( authorizationView == null )
			{
				MessageBox.Show( "Absolutely unexpected fatal error!", "Error" );
				return;	
			}

			PasswordBox box = authorizationView.passwordBox;

			OracleAccessor accessor = null;

			try
			{
				accessor = new OracleAccessor( _dataSource, _userName, box.Password, _tablespace );
			}
			catch ( Exception e )
			{
				int i = 5;
			}
			
			
			_manager.ShowWindow( new MainViewModel( _manager, accessor ) );
		}
	}
}
