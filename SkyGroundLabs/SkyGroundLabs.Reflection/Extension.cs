using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
	public static class Extension
	{
		public static T GetCustomAttribute<T>(this PropertyInfo property) where T : Attribute
		{
			var result = property.GetCustomAttributes(typeof(T), true).FirstOrDefault();

		    return result == null ? default(T) : (T) result;
		}

		public static List<T> GetCustomAttributes<T>(this PropertyInfo property) where T : Attribute
		{
			var find = property.GetCustomAttributes(typeof(T), true).ToList();

		    return find.Cast<T>().ToList();
		}

		public static T GetCustomAttribute<T>(this Type type) where T : Attribute
		{
			var result = type.GetCustomAttributes(typeof(T), true).FirstOrDefault();

            return result == null ? default(T) : (T)result;
		}
	}
}
