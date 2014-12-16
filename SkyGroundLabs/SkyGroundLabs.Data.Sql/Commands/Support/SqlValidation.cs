using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Enumeration;
using SkyGroundLabs.Data.Sql.Commands.Secure;

namespace SkyGroundLabs.Data.Sql.Commands.Support
{
	/// <summary>
	/// Builds the WHERE statement for queries
	/// </summary>
	public abstract class SqlValidation : SqlSecureExecutable
	{
		#region Properties
		private string _where { get; set; }
		#endregion

		#region Constructor
		public SqlValidation()
			: base()
		{
			_where = string.Empty;
		}
		#endregion

		#region Methods
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
			var startValidationString = " {0} [{1}].[{2}] {3} {4}{5}{6} ";

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
				case ComparisonType.EqualsIgnoreCase:
					startValidationString = " {0} UPPER([{1}].[{2}]) {3} UPPER({4}{5}{6}) ";
					break;
				case ComparisonType.EqualsTruncateTime:
					startValidationString = " {0} Cast([{1}].[{2}] as date) {3} Cast({4}{5}{6} as date) ";
					break;
			}

			var data = GetNextParameter();
			_where += string.Format(startValidationString, _getValidationType(), table, field, comparisonType, startComparisonType, data, endComparisonType);
			AddParameter(equals);
		}
		#endregion
	}
}
