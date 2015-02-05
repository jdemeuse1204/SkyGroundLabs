using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql
{
	public interface IDbTable
	{
		void Add(object entity);

		bool Remove(object entity);

		void Clear();

		object Where(Expression<Func<object, bool>> propertyLambda);

		object FirstOrDefault(Expression<Func<object, bool>> propertyLambda);

		IList<object> All();
	}
}
