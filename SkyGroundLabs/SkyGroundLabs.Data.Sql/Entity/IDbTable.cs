using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SkyGroundLabs.Data.Sql.Entity
{
	public interface IDbTable<T> 
	{
		void Add(T entity);

		bool Remove(T entity);

		void Clear();

		object Where(Expression<Func<T, bool>> propertyLambda);

		object FirstOrDefault(Expression<Func<T, bool>> propertyLambda);

		IList<T> All();
	}
}
