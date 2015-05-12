using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Caliburn.Micro;
using Komissarov.Nsu.OracleClient.Accessor;

namespace Komissarov.Nsu.OracleClient.ViewModels
{
	class MainViewModel : PropertyChangedBase
	{
		private IWindowManager _manager;
		private OracleAccessor _accessor;

		public string Stub
		{
			set;
			get;
		}

		public MainViewModel( IWindowManager manager, OracleAccessor accessor )
		{
			_manager = manager;
			_accessor = accessor;
			foreach ( var name in accessor.GetAvaliableTables( ) )
				Stub += name + "\n";
		}
	}
}
