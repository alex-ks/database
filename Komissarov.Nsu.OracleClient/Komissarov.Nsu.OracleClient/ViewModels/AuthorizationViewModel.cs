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
			get
			{
				return _userName;
			}
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

		public string Password
		{
			get
			{
				var authorizationView = GetView( ) as AuthorizationView;

				if ( authorizationView == null )
					throw new ArgumentException( "Can't get the password" );

				return authorizationView.passwordBox.Password;
			}
		}

		public OracleAccessor Accessor
		{
			get;
			private set;
		}

		public AuthorizationViewModel( )
		{
			DataSource = "10.4.0.119";
			UserName = "12201";
			Accessor = null;
		}

		public void LogIn( )
		{
			try
			{
				Accessor = new OracleAccessor( DataSource, UserName, Password, Tablespace );
			}
			catch ( Exception e )
			{
				MessageBox.Show( e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
			}
			
			TryClose( );
		}
	}
}
