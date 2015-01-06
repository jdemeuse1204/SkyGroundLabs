using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Commands.Secure
{
	public class SqlSecureObject
	{
		public SqlSecureObject(object value)
		{
			TranslateDataType = false;
			_value = value;
		}

		public SqlSecureObject(object value, SqlDbType type)
		{
			TranslateDataType = true;
			DbTranslationType = type;
			_value = value;
		}

		public bool TranslateDataType { get; private set; }

		public SqlDbType DbTranslationType { get; set; }

		private object _value { get; set; }
		public object Value 
		{
			get { return (_value == null ? DBNull.Value : _value); }
			set { _value = value; }
		}
	}
}
