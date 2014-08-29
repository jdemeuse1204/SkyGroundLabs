using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Linq.Mapping;

namespace SkyGroundLabs.Data.Linq
{
	public static class Extension
	{
		public static TEntity Find<TEntity, TPKType>(this Table<TEntity> table, TPKType ID, string pkName = "ID")
			where TEntity : DbTableEquatable<IDbTableEquatable<TPKType>>
			where TPKType : struct
		{
			TEntity copy = Activator.CreateInstance<TEntity>();

			// set value through reflection
			copy.GetType().GetProperty(pkName).SetValue(copy, ID, null);
			return (TEntity)table.Where(w => w.Equals(copy)).FirstOrDefault();
		}

		public static TEntity Find<TEntity>(this Table<TEntity> table, long ID, string pkName = "ID")
			where TEntity : DbTableEquatable<IDbTableEquatable<long>>
		{
			TEntity copy = Activator.CreateInstance<TEntity>();

			// set value through reflection
			copy.GetType().GetProperty(pkName).SetValue(copy, ID, null);
			return (TEntity)table.Where(w => w.Equals(copy)).FirstOrDefault();
		}
	}
}
