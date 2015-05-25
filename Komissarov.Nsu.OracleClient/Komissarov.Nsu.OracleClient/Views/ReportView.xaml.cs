﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Komissarov.Nsu.OracleClient.Views
{
	/// <summary>
	/// Interaction logic for ReportView.xaml
	/// </summary>
	public partial class ReportView : Window
	{
		public ReportView( )
		{
			InitializeComponent( );
		}

		private void DataGrid_AutoGeneratingColumn( object sender, DataGridAutoGeneratingColumnEventArgs e )
		{
			string header = e.Column.Header.ToString( );
			e.Column.Header = header.Replace( "_", "__" );
		}
	}
}
