using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public static class Extension
	{
		/// <summary>
		/// Converts a SqlDataReader to an object.  The return column names must match the properties names for it to work
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static T ToObject<T>(this SqlDataReader reader, params string[] namesToSkip)
		{
			T obj = Activator.CreateInstance<T>();

			foreach (var property in obj.GetType().GetProperties())
			{
				if (namesToSkip.Contains(property.Name))
				{
					continue;
				}

				property.SetValue(obj, reader[property.Name], null);
			}

			return obj;
		}

		/// <summary>
		/// Turns the DataReader into an object and converts the types for you
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static dynamic ToObject(this SqlDataReader reader)
		{
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
