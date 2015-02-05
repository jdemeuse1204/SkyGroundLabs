using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SkyGroundLabs.Data.Sql.Entity
{
    public class DbTable<T> : IDbTable<T> where T : class 
    {
        protected string tableName { get; set; }
        private DbSqlContext _context;
		private IList<T> _collection;

		public DbTable(DbSqlContext context)
		{
			_context = context;
			_collection = new List<T>();
		    tableName = Activator.CreateInstance<T>().GetDatabaseTableName();
		}

		public void Add(T entity)
		{
			_collection.Add(entity);
		}

		public bool Remove(T entity)
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

		public IList<T> All()
		{
            // loop through properties because names might be different, ie Test as State
			_context.Execute(string.Format("Select * FROM {0}", tableName));

			return _context.SelectList<T>();
		}
    }
}
