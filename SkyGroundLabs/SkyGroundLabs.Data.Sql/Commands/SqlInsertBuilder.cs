using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Support;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class SqlInsertBuilder : SqlSecureExecutable, ISqlBuilder
	{
		#region Properties
		private string _table { get; set; }
		private string _fields { get; set; }
		private string _values { get; set; }
		private bool _isIdentityInsertOn { get; set; }
		#endregion

		#region Constructor
		public SqlInsertBuilder(bool isIdentityInsertOn = true)
			: base()
		{
			_table = string.Empty;
			_fields = string.Empty;
			_values = string.Empty;
			_isIdentityInsertOn = isIdentityInsertOn;
		}
		#endregion

		#region Methods
		public SqlCommand BuildCommand(SqlConnection connection)
		{
			if (string.IsNullOrWhiteSpace(_table))
			{
				throw new QueryNotValidException("INSERT statement needs Table Name");
			}

			if (string.IsNullOrWhiteSpace(_values))
			{
				throw new QueryNotValidException("INSERT statement needs VALUES");
			}

			if (string.IsNullOrWhiteSpace(_fields))
			{
				throw new QueryNotValidException("INSERT statement needs Fields");
			}

			var identity = _isIdentityInsertOn ? ";Select @@IDENTITY;" : "";
			var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2}){3}", _table, _fields.TrimEnd(','), _values.TrimEnd(','), identity);
			var cmd = new SqlCommand(sql, connection);

			InsertParameters(cmd);

			return cmd;
		}

		public void Table(string tableName)
		{
			_table = tableName;
		}

		public void AddInsert(string fieldName, object value)
		{
			var data = GetNextParameter();
			_fields += string.Format("[{0}],", fieldName);
			_values += string.Format("{0},", data);
			AddParameter(value);
		}
		#endregion
	}
}
