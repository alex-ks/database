using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komissarov.Nsu.OracleClient.Models
{
	class Column
	{
		public string Name
		{
			set;
			get;
		}

		public string Type
		{
			set;
			get;
		}

		public bool Nullable
		{
			set;
			get;
		}

		public bool PrimaryKey
		{
			set;
			get;
		}

		public bool ForeignKey
		{
			set;
			get;
		}

		public string SourceTable
		{
			set;
			get;
		}

		public string SourceColumn
		{
			set;
			get;
		}
	}
}
