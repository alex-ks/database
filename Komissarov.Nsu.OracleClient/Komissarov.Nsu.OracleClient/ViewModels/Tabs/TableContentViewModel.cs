using Caliburn.Micro;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komissarov.Nsu.OracleClient.ViewModels.Tabs
{
	class TableContentViewModel : PropertyChangedBase, IConnected
	{
		private IAccessProvider _provider;
		private string _tableName;

		private DataTable _table;
		public DataTable TableContent
		{
			set
			{
				if ( _table == value )
					return;
				_table = value;
				NotifyOfPropertyChange( ( ) => TableContent );
			}
			get
			{
				return _table;
			}
		}

		public TableContentViewModel( IAccessProvider provider, string tableName )
		{
			_provider = provider;
			_tableName = tableName;

			if ( tableName != null )
				TableContent = _provider.Accessor.GetTableContent( _tableName );
		}

		public void CommitChanges( )
		{
			try
			{
				_provider.Accessor.UpdateTableContent( _tableName, _table );
			}
			catch ( OracleException e )
			{
				_provider.ReportError( e.Message );
			}
		}

		public void RevertChanges( )
		{
			try
			{
				TableContent = _provider.Accessor.GetTableContent( _tableName );
			}
			catch( OracleException e )
			{
				_provider.ReportError( e.Message );
			}	
		}

		public event DisconnectHandler Disconnected;
	}
}
