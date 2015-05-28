using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komissarov.Nsu.OracleClient.Models
{
	class EditableColumn : Column
	{
		public override bool Created
		{
			protected set;
			get;
		}

		public bool NameChanged
		{
			private set;
			get;
		}

		private string _name, _originalName;
		public override string Name
		{
			set
			{
				if ( _name == value )
					return;
				_name = value;
				if ( _originalName == null )
					_originalName = value;
				if ( _originalName == value )
					NameChanged = false;
				else
					NameChanged = true;
				
			}
			get
			{
				return _name;
			}
		}

		public bool TypeChanged
		{
			private set;
			get;
		}

		private string _type, _originalType;
		public override string Type
		{
			set
			{
				if ( _type == value )
					return;
				_type = value;
				if ( _originalType == null )
					_originalType = value;
				if ( _originalType == value )
					TypeChanged = false;
				else
					TypeChanged = true;
			}
			get
			{
				return _type;
			}
		}

		public bool NullableChanged
		{
			private set;
			get;
		}

		private bool _nullable, _originalNullable, _nullCreated = true;
		public override bool Nullable
		{
			set
			{
				if ( _nullable == value )
					return;
				_nullable = value;
				if ( _nullCreated )
				{
					_originalNullable = value;
					_nullCreated = false;
				}
				if ( _originalNullable == value )
					NullableChanged = false;
				else
					NullableChanged = true;
			}
			get
			{
				return _nullable;
			}
		}

		public bool PrimaryChanged
		{
			private set;
			get;
		}

		private bool _primary, _originalPrimary, _primaryCreated = true;
		public override bool PrimaryKey
		{
			set
			{
				if ( _primary == value )
					return;
				_primary = value;
				if ( _primaryCreated )
				{
					_originalPrimary = value;
					_primaryCreated = false;
				}
				if ( _originalPrimary == value )
					PrimaryChanged = false;
				else
					PrimaryChanged = true;
			}
			get
			{
				return _primary;
			}
		}

		public bool ForeignChanged
		{
			private set;
			get;
		}

		private bool _foreign, _originalForeign, _foreignCreated = true;
		public override bool ForeignKey
		{
			set
			{
				if ( _foreign == value )
					return;
				_foreign = value;
				if ( _foreignCreated )
				{
					_foreignCreated = false;
					_originalForeign = value;
					return;
				}
				if ( _foreign == _originalForeign
					&& _sourceTable == _originalSourceTable
					&& _sourceColumn == _originalSourceColumn )
					ForeignChanged = false;
				else
					ForeignChanged = true;
			
			}
			get
			{
				return _foreign;
			}
		}

		public string FKName
		{
			set;
			get;
		}

		private string _sourceTable, _originalSourceTable;
		public override string SourceTable
		{
			set
			{
				if ( _sourceTable == value )
					return;
				_sourceTable = value;
				if ( _originalSourceTable == null )
				{
					_originalSourceTable = value;
					return;
				}
				if ( _foreign == _originalForeign
					&& _sourceTable == _originalSourceTable
					&& _sourceColumn == _originalSourceColumn )
					ForeignChanged = false;
				else
					ForeignChanged = true;
			}
			get
			{
				return _sourceTable;
			}
		}

		private string _sourceColumn, _originalSourceColumn;
		public override string SourceColumn
		{
			set
			{
				if ( _sourceColumn == value )
					return;
				_sourceColumn = value;
				if ( _originalSourceColumn == null )
				{
					_originalSourceColumn = value;
					return;
				}
				if ( _foreign == _originalForeign
					&& _sourceTable == _originalSourceTable
					&& _sourceColumn == _originalSourceColumn )
					ForeignChanged = false;
				else
					ForeignChanged = true;
			}
			get
			{
				return _sourceColumn;
			}
		}

		public EditableColumn( )
		{
			Created = false;
			NameChanged = false;
			TypeChanged = false;
			PrimaryChanged = false;
			ForeignChanged = false;
		}
	}
}
