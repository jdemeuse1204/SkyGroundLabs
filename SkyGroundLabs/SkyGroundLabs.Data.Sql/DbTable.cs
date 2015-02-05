using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql
{
	public class DbTable : IDbTable
	{
		private string _tableName;
		private DbSqlContext _context;
		private IList<object> _collection;

		public DbTable(DbSqlContext context)
		{
			_context = context;
			_collection = new List<object>();
		}

		public void Add(object entity)
		{
			_collection.Add(entity);
		}

		public bool Remove(object entity)
		{
			return _collection.Remove(entity);
		}

		public void Clear()
		{
			_collection.Clear();
		}

		public object Where(Expression<Func<T, bool>> propertyLambda)
		{
			return null;
		}

		public object FirstOrDefault(Expression<Func<T, bool>> propertyLambda)
		{
			return null;
		}

		public object Find(params object[] pks)
		{
			return null;
		}

		public IList<object> All()
		{
			_context.Execute(string.Format("Select * FROM {0}", _tableName));

			return _context.SelectList<T>();
		}
	}
}
