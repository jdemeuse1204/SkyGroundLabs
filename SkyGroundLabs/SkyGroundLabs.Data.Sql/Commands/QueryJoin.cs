using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Functions;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class QueryJoin
	{
		public string Table { get; private set; }
		private Dictionary<string, List<string>> _values { get; set; }

		public QueryJoin(string tableName)
		{
			Table = tableName;
			_values = new Dictionary<string, List<string>>();
		}

		public void On(string parentFieldName, string childFieldName)
		{

		}

		public void Select(string fieldName)
		{
			_values.Add(fieldName, null);
		}

		public void Select(string fieldName, IDbFunction function)
		{
			var fns = new List<string>();
			fns.Add(function.Get());
			_values.Add(fieldName, fns);
		}

		public void Select(string fieldName, Dictionary<int,IDbFunction> functions)
		{
			var fns = new List<string>();

			foreach (var item in functions)
			{
				fns.Add(item.Value.Get());
			}

			_values.Add(fieldName, fns);
		}

		public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
		{
			return _values.GetEnumerator();
		}
	}
}
