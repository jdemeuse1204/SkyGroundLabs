using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Reflection
{
	public class ReflectionManager
	{
		public static void SetValues(object fromEntity, object toEntity)
		{
			_setValues(fromEntity, toEntity, null, null);
		}

		public static object GetPropertyValue(object entity, string propertyName)
		{
			var foundProperty = entity.GetType().GetProperty(propertyName);

			if (foundProperty != null)
			{
				return foundProperty.GetValue(entity);
			}

			return null;
		}

		private static void _setValues(object fromEntity, object toEntity, List<string> propertyNameToSkip, List<string> propertyNameToSet)
		{
			PropertyInfo[] properties = toEntity.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				if (propertyNameToSkip != null && propertyNameToSkip.Contains(property.Name))
				{
					continue;
				}

				if (propertyNameToSet == null || propertyNameToSet.Count == 0 ||
					propertyNameToSet.Contains(property.Name))
				{
					var foundProperty = fromEntity.GetType().GetProperty(property.Name);

					if (foundProperty != null)
					{
						var data = foundProperty.GetValue(fromEntity);
						toEntity.GetType().GetProperty(property.Name).SetValue(toEntity, data, null);
					}
				}
			}
		}

		public static PropertyInfo[] GetProperties(object entity)
		{
			return entity.GetType().GetProperties();
		}

		public static void SetPropertyValue(object entity, string propertyName, object value)
		{
			var found = entity.GetType().GetProperty(propertyName);

			if (found != null)
			{
				Type propertyType = found.PropertyType;

				//Nullable properties have to be treated differently, since we 
				//  use their underlying property to set the value in the object
				if (propertyType.IsGenericType
					&& propertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
				{
					//if it's null, just set the value from the reserved word null, and return
					if (value == null)
					{
						found.SetValue(entity, null, null);
						return;
					}

					//Get the underlying type property instead of the nullable generic
					propertyType = new NullableConverter(found.PropertyType).UnderlyingType;
				}

				//use the converter to get the correct value
				found.SetValue(
					entity,
					propertyType.IsEnum ? Enum.ToObject(propertyType, value) : Convert.ChangeType(value, propertyType),
					null);
			}
		}

		public static void SetValuesWithSkip(object fromEntity, object toEntity, params string[] propertyNameToSkip)
		{
			_setValues(fromEntity, toEntity, propertyNameToSkip.ToList(), null);
		}

		public static void SetValuesWithInclude(object fromEntity, object toEntity, params string[] propertyNameToInclude)
		{
			_setValues(fromEntity, toEntity, null, propertyNameToInclude.ToList());
		}

		public static T Clone<T>(object entity, CloneMode mode, params string[] propertyNames)
		{
			if (mode == CloneMode.Include)
			{
				T instance = Activator.CreateInstance<T>();
				SetValuesWithInclude(entity, instance, propertyNames);
				return instance;
			}
			else if (mode == CloneMode.Skip)
			{
				T instance = Activator.CreateInstance<T>();
				SetValuesWithSkip(entity, instance, propertyNames);
				return instance;
			}
			else
			{
				T instance = Activator.CreateInstance<T>();
				SetValues(entity, instance);
				return instance;
			}
		}
	}
}
