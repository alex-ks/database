using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Oracle.ManagedDataAccess.Client;
using System.Windows;

namespace Komissarov.Nsu.OracleClient.ViewModels.Tabs
{
	class TableBrowserViewModel : PropertyChangedBase, IConnected
	{
		private IAccessProvider _provider;
		private IWindowManager _manager;

		public IEnumerable<string> TableNames
		{
			get
			{
				if ( _provider.Accessor == null )
					return null;
				return _provider.Accessor.GetAvaliableTables( ).Where( str => SearchCriteria == null || str.ToLower( ).Contains( SearchCriteria.ToLower( ) ) );
			}
		}

		private string _item;
		public string SelectedItem
		{
			set
			{
				if ( _item == value )
					return;
				_item = value;
				NotifyOfPropertyChange( ( ) => SelectedItem );

				if ( _item == null )
				{
					TableContent = null;
					TableEditor = null;
					return;
				}
					
				try
				{
					TableEditor = new TableEditorViewModel( _provider, _item );
					TableEditor.TableEdited += Update;
					TableContent = new TableContentViewModel( _provider, _item );
				}
				catch( OracleException e )
				{
					SelectedItem = null;
					_provider.ReportError( e.Message );
				}
			}
			get
			{
				return _item;
			}
		}

		private string _criteria;
		public string SearchCriteria
		{
			set
			{
				if ( value == _criteria )
					return;

				_criteria = value;
				NotifyOfPropertyChange( ( ) => SearchCriteria );
				NotifyOfPropertyChange( ( ) => TableNames );
			}

			get
			{
				return _criteria;
			}
		}

		private TableContentViewModel _tableContent;
		public TableContentViewModel TableContent
		{
			set
			{
				if ( value == _tableContent )
					return;
				_tableContent = value;
				NotifyOfPropertyChange( ( ) => TableContent );
			}
			get
			{
				return _tableContent;
			}
		}

		private TableEditorViewModel _tableEditor;
		public TableEditorViewModel TableEditor
		{
			set
			{
				if ( value == _tableEditor )
					return;
				_tableEditor = value;
				NotifyOfPropertyChange( ( ) => TableEditor );
			}
			get
			{
				return _tableEditor;
			}
		}

		public TableBrowserViewModel( IWindowManager manager, IAccessProvider provider )
		{
			_provider = provider;
			_manager = manager;
			provider.ConnectEvent += ConnectHander;
		}

		public void AddTable( )
		{
			TableEditor = new TableEditorViewModel( _provider, null );
			TableEditor.TableEdited += Update;
			TableContent = new TableContentViewModel( _provider, null );
		}

		public void DeleteTable( )
		{
			if ( SelectedItem == null )
				return;

			if ( MessageBox.Show( "Are you sure you want to delete table " + SelectedItem + '?', "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question ) == MessageBoxResult.Yes )
			{
				try
				{
					_provider.Accessor.ExecuteQuery( "DROP TABLE " + SelectedItem );
					MessageBox.Show( "Table " + SelectedItem + " has been successfully deleted", "Report", MessageBoxButton.OK, MessageBoxImage.Information );
					SelectedItem = null;
					Update( );
				}
				catch ( OracleException e )
				{
					_provider.ReportError( e.Message );
				}
			}
		}

		private void Update( )
		{
			_provider.Accessor.ResetAllLists( );
			NotifyOfPropertyChange( ( ) => TableNames );
		}

		public void ConnectHander( )
		{
			SelectedItem = null;
			SearchCriteria = null;
			TableContent = null;
			TableEditor = null;
			Update( );
		}

		public event DisconnectHandler Disconnected;
	}
}
