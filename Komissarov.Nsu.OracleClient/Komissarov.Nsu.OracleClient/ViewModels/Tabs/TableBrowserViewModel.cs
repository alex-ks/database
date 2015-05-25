﻿using System;
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
					return;
				try
				{
					TableContent = new TableContentViewModel( _provider, _item );
				}
				catch( OracleException e )
				{
					TableContent = null;
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

		public TableBrowserViewModel( IWindowManager manager, IAccessProvider provider )
		{
			_provider = provider;
			_manager = manager;
			provider.ConnectEvent += ConnectHander;
		}

		public void ConnectHander( )
		{
			SelectedItem = null;
			SearchCriteria = null;
			TableContent = null;
			NotifyOfPropertyChange( ( ) => TableNames );
		}

		public event DisconnectHandler Disconnected;
	}
}
