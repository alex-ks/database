using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komissarov.Nsu.OracleClient
{
	public delegate void DisconnectHandler( );

	interface IConnected
	{
		event DisconnectHandler Disconnected;
	}
}
