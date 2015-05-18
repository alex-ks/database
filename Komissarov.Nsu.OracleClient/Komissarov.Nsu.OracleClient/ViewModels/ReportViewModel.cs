using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Oracle.ManagedDataAccess.Client;

namespace Komissarov.Nsu.OracleClient.ViewModels
{
	class ReportViewModel : PropertyChangedBase
	{
		public OracleDataReader DataReader
		{
			get;
			set;
		}

		public ReportViewModel( OracleDataReader reader )
		{
			DataReader = reader;
		}
	}
}
