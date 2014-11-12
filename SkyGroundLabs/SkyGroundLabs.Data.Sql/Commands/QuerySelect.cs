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
		private Dictionary<string, List<string>> _values { get; set; } // field name and functions
		private Dictionary<string, string> _validation { get; set; }
		private List<QueryEquiJoin> _joins { get; set; }
		public int Count { get { return _values.Count; } }

		public QuerySelect(string tableName)
		{
			Table = tableName;
			_values = new Dictionary<string, List<string>>();
			_validation = new Dictionary<string, string>();
			_joins = new List<QueryEquiJoin>();
		}

		public void Select(string fieldName)
		{
			_values.Add(fieldName, null);
		}

		public void Select(params string[] fieldNames)
		{
			foreach (var fieldName in fieldNames)
			{
				_values.Add(fieldName, null);
			}
		}

		public void SelectAll()
		{
			_values = new Dictionary<string, List<string>>();
		}

		public void Where(string fieldName, string value)
		{
			_validation.Add(fieldName, value);
		}

		public void Join(QueryEquiJoin join)
		{
			_joins.Add(join);
		}

		public IEnumerable<KeyValuePair<string, string>> GetValidation()
		{
			return _validation;
		}

		public IEnumerable<QueryEquiJoin> GetJoins()
		{
			return _joins;
		}

		public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
		{
			return _values.GetEnumerator();
		}
	}
}
