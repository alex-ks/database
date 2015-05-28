using System;
using System.Windows;
using Caliburn.Micro;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Win32;
using System.IO;

namespace Komissarov.Nsu.OracleClient.ViewModels.Tabs
{
	class QueryViewModel : PropertyChangedBase, IConnected
	{
		private IAccessProvider _provider;
		private IWindowManager _manager;
		private string _query;

		public event UpdateHandler RequireUpdate;

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
				ReportViewModel report = new ReportViewModel( dataReader );
				_manager.ShowDialog( report );
				if ( RequireUpdate != null )
					RequireUpdate( );
			}
			catch ( OracleException e )
			{
				_provider.ReportError( e.Message );
			}
			catch ( InvalidOperationException )
			{
				_provider.ReportError( "Invalid query text" );
			}
			catch ( NullReferenceException )
			{
				_provider.ReportError( "Connection error" );
			}	
		}

		public void SaveQuery( )
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog( );
			saveFileDialog.DefaultExt = "sql";
			saveFileDialog.Filter = "SQL script|*.sql";
			saveFileDialog.AddExtension = true;

			if ( saveFileDialog.ShowDialog( ) == true )
				File.WriteAllText( saveFileDialog.FileName, Query );
		}

		public void LoadQuery( )
		{
			OpenFileDialog openFileDialog = new OpenFileDialog( );
			openFileDialog.Filter = "SQL script|*.sql|All files|*";
			openFileDialog.ValidateNames = true;

			if ( openFileDialog.ShowDialog( ) == true )
				Query = File.ReadAllText( openFileDialog.FileName );
		}
	}
}
