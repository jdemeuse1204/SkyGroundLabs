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

		public static List<object> GetCustomAttributes<T>(this PropertyInfo property) where T : Attribute
		{
			var result = property.GetCustomAttributes(typeof(T), true).ToList();

			if (result == null)
			{
				return null;
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
