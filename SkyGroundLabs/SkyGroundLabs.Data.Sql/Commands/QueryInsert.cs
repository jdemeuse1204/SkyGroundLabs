using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class QueryInsert
	{
		public string Table { get; private set; }
		private Dictionary<string, string> _values { get; set; }

		public QueryInsert(string tableName)
		{
			Table = tableName;
			_values = new Dictionary<string, string>();
		}

		public void Insert(string fieldName, string value)
		{
			_values.Add(fieldName, value);
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return _values.GetEnumerator();
		}
	}
}
