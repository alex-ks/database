using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Komissarov.Nsu.OracleClient.Accessor
{
	class StringLoader
	{
		public string InfoGettingQuery
		{
			get;
			private set;
		}
		public string TypeGettingQuery
		{
			get;
			private set;
		}

		public StringLoader( )
		{
			XmlDocument document = new XmlDocument( );
			document.Load( "strings.xml" );
			var list = document.GetElementsByTagName( "string" );

			InfoGettingQuery = list[0].InnerText;
			TypeGettingQuery = list[1].InnerText;
		}

	}
}
