using System;
using System.Data.SqlClient;
using System.Reflection;
using SkyGroundLabs.Data.Sql.Commands.Support;
using SkyGroundLabs.Data.Sql.Data;
using SkyGroundLabs.Data.Sql.Mapping;

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
		public SqlCommand Build(SqlConnection connection)
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

		public void AddUpdate(PropertyInfo property, object entity)
		{
			//string fieldName, object value
			var value = property.GetValue(entity);
            var fieldName = DatabaseSchemata.GetColumnName(property);
			var data = GetNextParameter();
			_set += string.Format("[{0}] = {1},", fieldName, data);

			// check for sql data translation, used mostly for datetime2 inserts and updates
			var translation = property.GetCustomAttribute<DbTranslationAttribute>();

			if (translation != null)
			{
				AddParameter(value, translation.Type);
			}
			else
			{
				AddParameter(value);
			}
		}
		#endregion
	}
}
