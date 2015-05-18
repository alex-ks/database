using System;
using System.Windows;
using Caliburn.Micro;
using Oracle.ManagedDataAccess.Client;

namespace Komissarov.Nsu.OracleClient.ViewModels.Tabs
{
	class QueryViewModel : PropertyChangedBase, IConnected
	{
		private IAccessProvider _provider;
		private IWindowManager _manager;
		private string _query;

		public event DisconnectHandler Disconnected;

		public string Query
		{
			get { return _query; }
			set
			{
				if ( value.Equals( _query ) )
					return;

				_query = value;
				NotifyOfPropertyChange( ( ) => Query );
			}
		}

		public QueryViewModel( IWindowManager manager, IAccessProvider provider )
		{
			_manager = manager;
			_provider = provider;
		}

		public void ExecuteQuery( )
		{
			try
			{
				var dataReader = _provider.Accessor.ExecuteQuery( Query );
				//todo: report creation
				dataReader.Dispose( );
			}
			catch ( OracleException e )
			{
				MessageBox.Show( e.Message, "Error" );
				if ( Disconnected != null )
					Disconnected( );
			}
			catch ( InvalidOperationException )
			{
				MessageBox.Show( "Invalid query text", "Error" );
			}
			catch ( NullReferenceException )
			{
				MessageBox.Show( "Connection error", "Error" );
			}	
		}
	}
}
