using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Functions;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class QuerySelect
	{
		public string Table { get; private set; }
		private Dictionary<string, List<string>> _values { get; set; }
		private Dictionary<string, string> _validation { get; set; }
		private List<QueryJoin> _joins { get; set; }

		public QuerySelect(string tableName)
		{
			Table = tableName;
			_values = new Dictionary<string, List<string>>();
			_validation = new Dictionary<string, string>();
			_joins = new List<QueryJoin>();
		}

		public void Select(string fieldName)
		{
			_values.Add(fieldName, null);
		}

		public void Where(string fieldName, string value)
		{
			_validation.Add(fieldName, value);
		}

		public void Join(QueryJoin join)
		{
			_joins.Add(join);
		}

		public IEnumerable<KeyValuePair<string, string>> GetValidation()
		{
			return _validation;
		}

		public IEnumerable<QueryJoin> GetJoins()
		{
			return _joins;
		}

		public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
		{
			return _values.GetEnumerator();
		}
	}
}
