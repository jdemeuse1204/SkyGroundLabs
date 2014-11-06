using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class QueryUpdate
	{
		public string Table { get; private set; }
		private Dictionary<string, string> _values { get; set; }
		private Dictionary<string, string> _validation { get; set; }

		public QueryUpdate(string tableName)
		{
			Table = tableName;
			_values = new Dictionary<string, string>();
			_validation = new Dictionary<string, string>();
		}

		public void Set(string fieldName, string value)
		{
			_values.Add(fieldName, value);
		}

		public void Where(string fieldName, string value)
		{
			_validation.Add(fieldName, value);
		}

		public IEnumerable<KeyValuePair<string, string>> GetValidation()
		{
			return _validation;
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return _values.GetEnumerator();
		}
	}
}
