using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Komissarov.Nsu.OracleClient.Accessor;

namespace Komissarov.Nsu.OracleClient
{
	public delegate void ConnectHandler( );

	interface IAccessProvider
	{
		event ConnectHandler ConnectEvent;

		OracleAccessor Accessor
		{
			get;
		}

		void ReportError( string message );
	}
}
