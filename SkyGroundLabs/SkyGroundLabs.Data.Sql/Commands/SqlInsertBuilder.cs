using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Support;
using SkyGroundLabs.Data.Sql.Enumeration;
using System.Reflection;
using SkyGroundLabs.Data.Sql.KeyGeneration;
using SkyGroundLabs.Data.Sql.Mapping;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class SqlInsertBuilder : SqlSecureExecutable, ISqlBuilder
	{
		#region Properties
		private string _table { get; set; }
		private string _fields { get; set; }
		private string _identity { get; set; }
		private string _values { get; set; }
		private string _sqlVariables { get; set; }
		#endregion

		#region Constructor
		public SqlInsertBuilder()
			: base()
		{
			_table = string.Empty;
			_fields = string.Empty;
			_values = string.Empty;
			_sqlVariables = string.Empty;
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

			var declare = !string.IsNullOrWhiteSpace(_sqlVariables) ? "DECLARE " + _sqlVariables.TrimEnd(',') : "";

			var set = string.Empty;

			if (!string.IsNullOrWhiteSpace(_sqlVariables))
			{
				foreach (var item in _sqlVariables.TrimEnd(',').Split(','))
				{
					var type = item.Replace(" as ",",").Split(',')[1].Trim();
					var variable = item.Replace(" as ", ",").Split(',')[0].Trim();
					var keyGeneration = string.Format("Select ISNULL(MAX({0}),0) + 1 From {1}",
						variable.Replace("@", ""), 
						_table);

					if (type.ToUpper() == "UNIQUEIDENTIFIER")
					{
						keyGeneration = "NEWID()";
					}

					set += string.Format("SET {0} = {1};", variable, keyGeneration);
				}
			}

			var sql = string.Format("{0} {1} INSERT INTO {2} ({3}) VALUES ({4});{5}",
				declare,
				set,
				_table, _fields.TrimEnd(','),
				_values.TrimEnd(','),
				string.IsNullOrWhiteSpace(_identity) ? "" : string.Format("Select {0}", _identity.TrimEnd(',')));

			var cmd = new SqlCommand(sql, connection);

			InsertParameters(cmd);

			return cmd;
		}

		public void Table(string tableName)
		{
			_table = tableName;
		}

		public void AddInsert(PropertyInfo property)
		{
			var propertyName = property.Name;
			var dbColumnName = property.GetDatabaseColumnName();
			var propertyValue = property.GetValue(entity);
			var variable = string.Empty;
			var sqlDataType = "int";
			var dbGenerationColumn = property.GetCustomAttribute<DbGenerationOptionAttribute>();
			var dbGenerationType = DbGenerationType.None;

			// make sure the property is not null
			propertyValue = propertyValue ?? "NULL";

			if (dbGenerationColumn != null)
			{
				dbGenerationType = dbGenerationColumn.Option;
			}

			// make sure the method is used properly
			if (dbGenerationType == DbGenerationType.None)
			{
				var data = GetNextParameter();
				_fields += string.Format("[{0}],", dbColumnName);
				_values += string.Format("{0},", data);
				AddParameter(propertyValue);
				return;
			}
			else if (dbGenerationType == DbGenerationType.Generate)
			{
				// need to automatically generate our key
				var key = string.Format("@{0}", propertyName);
				variable = string.Format("{0} as {1}", key, propertyName);

				// set the sql data type
				switch (propertyValue.GetType().Name.ToUpper())
				{
					case "INT16":
						sqlDataType = "smallint";
						break;
					case "INT64":
						sqlDataType = "bigint";
						break;
					case "GUID":
						sqlDataType = "uniqueidentifier";
						break;
				}
				_sqlVariables += string.Format("{0} as {1},", key, sqlDataType);
				_identity += variable + ",";
			}
			else
			{
				// identity specification is on
				_identity = string.Format("@@IDENTITY as {0},", propertyName);
			}



		}
		#endregion
	}
}
