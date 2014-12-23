using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public static class Extension
	{
		public static T GetCustomAttribute<T>(this PropertyInfo property) where T : Attribute
		{
			var result = property.GetCustomAttributes(typeof(T), true).FirstOrDefault();

			if (result == null)
			{
				return default(T);
			}

			return (T)result;
		}

		public static List<T> GetCustomAttributes<T>(this PropertyInfo property) where T : Attribute
		{
			var find = property.GetCustomAttributes(typeof(T), true).ToList();
			var result = new List<T>();

			if (find == null)
			{
				return new List<T>();
			}

			foreach (var item in find)
			{
				result.Add((T)item);
			}

			return result;
		}

		public static T GetCustomAttribute<T>(this Type type) where T : Attribute
		{
			var result = type.GetCustomAttributes(typeof(T), true).FirstOrDefault();

			if (result == null)
			{
				return default(T);
			}

			return (T)result;
		}
	}
}
