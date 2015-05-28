using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komissarov.Nsu.OracleClient.Models
{
	class Column
	{
		public virtual bool Created
		{
			set;
			get;
		}

		public virtual string Name
		{
			set;
			get;
		}

		public virtual string Type
		{
			set;
			get;
		}

		public virtual string Length
		{
			set;
			get;
		}

		public virtual bool Nullable
		{
			set;
			get;
		}

		public virtual bool PrimaryKey
		{
			set;
			get;
		}

		public virtual bool ForeignKey
		{
			set;
			get;
		}

		public virtual string SourceTable
		{
			set;
			get;
		}

		public virtual string SourceColumn
		{
			set;
			get;
		}

		public Column( )
		{
			Created = true;
		}
	}
}
