using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Komissarov.Nsu.OracleClient.Accessor;

namespace Komissarov.Nsu.OracleClient
{
	interface IAccessProvider
	{
		OracleAccessor Accessor
		{
			get;
		}
	}
}
