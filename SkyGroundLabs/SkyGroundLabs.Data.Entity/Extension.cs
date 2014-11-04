using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace SkyGroundLabs.Data.Entity
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
		/// This is depreciated, please use Entity Frameworks Find Command.
		/// This was for Linq.Data.Sql because it does not have a find command.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TPKType"></typeparam>
		/// <param name="table"></param>
		/// <param name="ID"></param>
		/// <param name="pkName"></param>
		/// <returns></returns>
		public static TEntity Find<TEntity, TPKType>(this IDbSet<TEntity> table, TPKType ID, string pkName = "ID")
			where TEntity : DbTableEquatable<IDbTableEquatable<TPKType>>
			where TPKType : struct
		{
			TEntity copy = Activator.CreateInstance<TEntity>();

			// set value through reflection
			copy.GetType().GetProperty(pkName).SetValue(copy, ID, null);
			return (TEntity)table.Where(w => w.Equals(copy)).FirstOrDefault();
		}
	}
}
