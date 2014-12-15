using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;
using SkyGroundLabs.Reflection;

namespace SkyGroundLabs
{
	public static class Extension
	{
		/// <summary>
		/// Converts a SqlDataReader to an object.  The return column names must match the properties names for it to work
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static T ToObject<T>(this SqlDataReader reader)
		{
			// Create instance
			T obj = Activator.CreateInstance<T>();

			// find any unmapped attributes
			var properties = obj.GetType().GetProperties().Where(w => w.GetCustomAttribute<UnmappedAttribute>() == null);

			// find any columns that have the column name attribute on them,
			// we need to swtich the column name to the one in the property
			var columnRenameProperties = obj.GetType().GetProperties().Where(w => w.GetCustomAttribute<ColumnAttribute>() != null).Select(w => w.Name);

			foreach (var property in properties)
			{
				var columnName = property.Name;

				if (columnRenameProperties.Contains(columnName))
				{
					columnName = property.GetCustomAttribute<ColumnAttribute>().Name;
				}

				var dbValue = reader[columnName];
				ReflectionManager.SetPropertyValue(obj, property.Name, dbValue is DBNull ? null : dbValue);
			}

			return obj;
		}

		/// <summary>
		/// Checks to see if the column is the primary key
		/// </summary>
		/// <param name="property">PropertyInfo</param>
		/// <returns>bool</returns>
		public static bool IsPrimaryKey(this PropertyInfo property)
		{
			return property.Name.ToUpper() == "ID "
				|| property.ToDatabaseColumnName().ToUpper() == "ID"
				|| property.GetCustomAttribute<KeyAttribute>() != null;
		}

		public static bool IsNumeric(this object o)
		{
			var result = 0L;

			return long.TryParse(o.ToString(), out result);
		}

		public  static string ToDatabaseColumnName(this PropertyInfo column)
		{
			var columnAttribute = column.GetCustomAttribute<ColumnAttribute>();

			return columnAttribute == null ? column.Name : columnAttribute.Name;
		}

		/// <summary>
		/// Turns the DataReader into an object and converts the types for you
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static dynamic ToObject(this SqlDataReader reader)
		{
			if (!reader.HasRows)
			{
				return null;
			}

			var result = new ExpandoObject() as IDictionary<string, Object>;

			IDataRecord rec = (IDataRecord)reader;

			for (int i = 0; i < rec.FieldCount; i++)
			{
				result.Add(rec.GetName(i), rec.GetValue(i));
			}

			return result;
		}
	}
}
