using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Commands.Support;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class SqlQueryBuilder : SqlValidation, ISqlBuilder
	{
		#region Properties
		private string _select { get; set; }
		private string _columns { get; set; }
		private string _from { get; set; }
		private string _table { get; set; }
		private Dictionary<string, object> _parameters { get; set; }
		#endregion

		#region Constructor
		public SqlQueryBuilder()
		{
			_select = string.Empty;
			_columns = string.Empty;
			_from = string.Empty;
			_table = string.Empty;
			_parameters = new Dictionary<string, object>();
		}
		#endregion

		#region Methods
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

		public void Select(string table, params Field[] fields)
		{
			_select = " SELECT ";

			foreach (var field in fields)
			{
				var alias = string.IsNullOrWhiteSpace(field.Alias) ? "" : string.Format(" AS {0}", field.Alias);
				_columns += string.Format("[{0}].[{1}]{2},", table, field.ColumnName, alias);
			}
		}

		public void SelectAll()
		{
			_select = " SELECT * ";
		}

		public void SelectAll<T>()
		{
			T instance = Activator.CreateInstance<T>();
			Table(instance.GetDatabaseTableName());
			instance = default(T);
			_select = " SELECT * ";
		}

		public void SelectTopOneAll()
		{
			_select = " SELECT TOP 1 * ";
		}

		public void SelectTop(int rows, string table, params Field[] fields)
		{
			_select = string.Format(" SELECT TOP {0} ", rows);

			foreach (var field in fields)
			{
				var alias = string.IsNullOrWhiteSpace(field.Alias) ? "" : string.Format(" AS {0}", field.Alias);
				_columns += string.Format("[{0}].[{1}]{2},", table, field.ColumnName, alias);
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
		#endregion
	}
}
