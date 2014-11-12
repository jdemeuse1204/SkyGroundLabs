using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Functions;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class QueryEquiJoin
	{
		public string ParentTable { get; private set; }
		public string ChildTable { get; private set; }
		private List<string> _values { get; set; }
		private Dictionary<string, string> _validation { get; set; }
		public string ParentTableJoinValue { get; private set; }
		public string ChildTableJoinValue { get; private set; }

		public QueryEquiJoin(string parentTableName, string childTableName)
		{
			ParentTable = parentTableName;
			ChildTable = childTableName;
			_values = new List<string>();
			_validation = new Dictionary<string, string>();
		}

		public void On(string parentTableJoinValue, string childTableJoinValue)
		{
			ParentTableJoinValue = parentTableJoinValue;
			ChildTableJoinValue = childTableJoinValue;
		}

		public void Where(string fieldName, string value)
		{
			_validation.Add(fieldName, value);
		}

		public void Select(string fieldName)
		{
			_values.Add(fieldName);
		}

		public IEnumerable<KeyValuePair<string, string>> GetValidation()
		{
			return _validation;
		}

		public IEnumerator<string> GetEnumerator()
		{
			return _values.GetEnumerator();
		}
	}
}
