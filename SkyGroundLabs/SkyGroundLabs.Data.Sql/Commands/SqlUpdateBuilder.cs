using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Commands.Support;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class SqlUpdateBuilder : SqlValidation, ISqlBuilder
	{
		#region Properties
		private string _table { get; set; }
		private string _set { get; set; }
		#endregion

		#region Constructor
		public SqlUpdateBuilder()
		{
			_table = string.Empty;
			_set = string.Empty;
		}
		#endregion

		#region Methods
		public SqlCommand BuildCommand(SqlConnection connection)
		{
			if (string.IsNullOrWhiteSpace(_table))
			{
				throw new QueryNotValidException("UPDATE table missing");
			}

			if (string.IsNullOrWhiteSpace(_set))
			{
				throw new QueryNotValidException("UPDATE SET values missing");
			}

			var sql = string.Format("UPDATE {0} SET {1} {2}", _table, _set.TrimEnd(','), GetValidation());
			var cmd = new SqlCommand(sql, connection);

			InsertParameters(cmd);

			return cmd;
		}

		public void Table(string tableName)
		{
			_table = tableName;
		}

		public void AddUpdate(string fieldName, object value)
		{
			var data = GetNextParameter();
			_set += string.Format("[{0}] = {1},", fieldName, data);
			AddParameter(value);
		}
		#endregion
	}
}
