using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Enumeration;

namespace SkyGroundLabs.Data.Sql.Support
{
	public abstract class SqlValidation : SqlSecureExecutable
	{
		private string _where { get; set; }

		public SqlValidation()
			: base()
		{
			_where = string.Empty;
		}

		public string GetValidation()
		{
			return _where;
		}

		public void AddWhere(string parentTable, string parentField, string childTable, string childField)
		{
			_where += string.Format(" {0} [{1}].[{2}] = [{3}].[{4}] ",
						_getValidationType(),
						parentTable,
						parentField,
						childTable,
						childField);
		}

		private string _getValidationType()
		{
			return _where.Contains("WHERE") ? "AND " : "WHERE ";
		}

		public void AddWhere(string table, string field, ComparisonType type, object equals)
		{
			var comparisonType = "=";
			var startComparisonType = "";
			var endComparisonType = "";

			switch (type)
			{
				case ComparisonType.Contains:
					startComparisonType = "'%";
					endComparisonType = "%'";
					comparisonType = "LIKE";
					break;
				case ComparisonType.BeginsWith:
					endComparisonType = "%'";
					comparisonType = "LIKE";
					break;
				case ComparisonType.EndsWith:
					startComparisonType = "'%";
					comparisonType = "LIKE";
					break;
			}

			var data = GetNextParameter();
			_where += string.Format(" {0} [{1}].[{2}] {3} {4}{5}{6} ", _getValidationType(), table, field, comparisonType, startComparisonType, data, endComparisonType);
			AddParameter(equals);
		}
	}
}
