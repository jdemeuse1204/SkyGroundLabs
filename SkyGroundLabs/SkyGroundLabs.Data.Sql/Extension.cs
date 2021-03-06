﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
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
            if (!reader.HasRows) return default(T);

			// Create instance
			var obj = Activator.CreateInstance<T>();

		    if (typeof (T).IsValueType)
		    {
		        obj = (T)reader[0];

		        return obj;
		    }

            if (typeof(T) == typeof(IDynamicMetaObjectProvider))
		    {
		        return reader.ToObject();
		    }

			// find any unmapped attributes
			var properties = obj.GetType().GetProperties().Where(w => w.GetCustomAttribute<UnmappedAttribute>() == null);

			// find any columns that have the column name attribute on them,
			// we need to swtich the column name to the one in the property
			var columnRenameProperties = obj.GetType().GetProperties().Where(w => w.GetCustomAttribute<ColumnAttribute>() != null).Select(w => w.Name).ToList();

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

		public static bool IsNumeric(this object o)
		{
			var result = 0L;

			return long.TryParse(o.ToString(), out result);
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

			var rec = (IDataRecord)reader;

			for (var i = 0; i < rec.FieldCount; i++)
			{
				result.Add(rec.GetName(i), rec.GetValue(i));
			}

			return result;
		}
	}

	public static class StringExtension
	{
		public static string ReplaceFirst(this string text, string search, string replace)
		{
			var pos = text.IndexOf(search);

			if (pos < 0)
			{
				return text;
			}

			return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
		}
	}
}
