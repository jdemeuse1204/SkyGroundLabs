using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace SkyGroundLabs.Data.Sql.KeyGeneration
{
	public class KeyGenerator
	{
		private string _sqlDeclare { get; set; }

		private List<string> _keys { get; set; }

		private object _entity { get; set; }

		private List<PropertyInfo> _keyGenerationColumns { get; set; }

		public KeyGenerator(object entity, List<PropertyInfo> keyGenerationColumns)
		{
			_entity = entity;
			_keyGenerationColumns = keyGenerationColumns;
			_keys = new List<string>();
		}

		public void Build()
		{
			var tableName = _entity.GetType().Name;

			// check for table name attribute
			var tableAttribute = _entity.GetType().GetCustomAttribute<TableAttribute>();

			// Set the table name 
			if (tableAttribute != null)
			{
				tableName = tableAttribute.Name;
			}

			// Once we are here the DbGenerationOption is Generate
			var sql = "DECLARE ";
			var set = "";

			for (int i = 0; i < _keyGenerationColumns.Count; i++)
			{
				var key = string.Format("@KEY{0}", i);
				var primaryKey = _keyGenerationColumns[i];
				var primaryKeyValue = primaryKey.GetValue(_entity);

				sql += string.Format("{0} as {1},", key, _getDatabaseType(primaryKeyValue));

				// in the insert script the key values will replace the actual values
				_keys.Add(key);

				// if its a guid we need the new key
				if (primaryKeyValue is Guid)
				{
					set += string.Format("Set {0} = NEWID();", key);
				}
				else if (primaryKeyValue.IsNumeric())
				{
					set += string.Format("Set {0} = (Select ISNULL(MAX({1}),0) + 1 From {2});", key, primaryKey.ToDatabaseColumnName(), tableName);
				}
				else
				{
					throw new Exception("Primary Key Type Must Be Numeric or Guid");
				}
			}

			sql = sql.TrimEnd(',') + " ";

			sql += set;

			_sqlDeclare = sql;
		}

		public string GetSqlDeclareKeyGeneration()
		{
			return _sqlDeclare;
		}

		public List<string> GetKeys()
		{
			return _keys;
		}

		private string _getDatabaseType(object primaryKeyValue)
		{
			var type = primaryKeyValue.GetType().Name.ToUpper();

			switch (type)
			{
				case "INT64":
					return "bigint";
				case "INT32":
					return "int";
				case "DECIMAL":
					return "decimal";
				case "DOUBLE":
					return "float";
				case "SINGLE":
					return "real";
				case "INT16":
					return "smallint";
				case "GUID":
					return "uniqueidentifier";
				default:
					return "";
			}
		}
	}
}
