using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Enumeration;
using SkyGroundLabs.Data.Sql.Support;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class SqlQueryBuilder : SqlValidation, ISqlBuilder
	{
		private string _select { get; set; }
		private string _columns { get; set; }
		private string _from { get; set; }
		private string _table { get; set; }
		private Dictionary<string, object> _parameters { get; set; }

		public SqlQueryBuilder()
			: base()
		{
			_select = string.Empty;
			_columns = string.Empty;
			_from = string.Empty;
			_table = string.Empty;
			_parameters = new Dictionary<string, object>();
		}

		public SqlCommand BuildCommand(SqlConnection connection)
		{
			if (string.IsNullOrWhiteSpace(_select))
			{
				throw new QueryNotValidException("SELECT statement missing");
			}

			if (string.IsNullOrWhiteSpace(_from))
			{
				throw new QueryNotValidException("FROM statement missing");
			}

			var sql = _select + _columns.TrimEnd(',') + _from + GetValidation();
			var cmd = new SqlCommand(sql, connection);

			InsertParameters(cmd);

			return cmd;
		}

		public void Table(string tableName)
		{
			_from += string.Format(" FROM {0} ", tableName);
		}

		public void Select(string table, params string[] fields)
		{
			_select = " SELECT ";

			foreach (var field in fields)
			{
				_columns += string.Format("[{0}].[{1}],", table, field);
			}
		}

		public void SelectAll()
		{
			_select = " SELECT * ";
		}

		public void SelectTop(int rows, string table, params string[] fields)
		{
			_select = string.Format(" SELECT TOP {0} ", rows);

			foreach (var field in fields)
			{
				_columns += string.Format("[{0}].[{1}],", table, field);
			}
		}

		public void AddJoin(JoinType type, string parentTable, string parentField, string childTable, string childField)
		{
			switch (type)
			{
				case JoinType.Equi:
					_from += string.Format(",{0}", childTable);
					AddWhere(parentTable, parentField, childTable, childField);
					break;
				case JoinType.Inner:
					_from += string.Format(" INNER JOIN [{0}] On [{1}].[{2}] = [{3}].[{4}] ",
						parentTable,
						parentTable,
						parentField,
						childTable,
						childField);
					break;
				case JoinType.Left:
					_from += string.Format(" LEFT JOIN [{0}] On [{1}].[{2}] = [{3}].[{4}] ",
						parentTable,
						parentTable,
						parentField,
						childTable,
						childField);
					break;
				default:
					break;
			}
		}
	}
}
