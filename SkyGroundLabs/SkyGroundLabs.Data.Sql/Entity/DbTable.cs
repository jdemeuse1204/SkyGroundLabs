using SkyGroundLabs.Data.Sql.Commands;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SkyGroundLabs.Data.Sql.Entity
{
    public class DbTable<T> : IDbTable<T> where T : class
    {
        public string TableName { get { return _tableName; } }
        private string _tableName { get; set; }
        public bool HasChanges
        {
            get { return _collection != null && _collection.Count > 0; }
        }
        private readonly DbSqlContext _context;
        private readonly Dictionary<T, SaveAction> _collection;

        // Expose for user, not editable
        public IEnumerable<KeyValuePair<T,SaveAction>> Local { get { return _collection; } } 

        public DbTable(DbSqlContext context)
        {
            _context = context;
            _collection = new Dictionary<T, SaveAction>();
            _tableName = Activator.CreateInstance<T>().GetDatabaseTableName();
        }

        public void Add(T entity)
        {
            Add(entity, SaveOption.None);
        }

        public void Add(T entity, SaveOption saveOption)
        {
            switch (saveOption)
            {
                case SaveOption.ForceInsert:
                    _collection.Add(entity, SaveAction.ForceInsert);
                    break;
                case SaveOption.ForceUpdate:
                    _collection.Add(entity, SaveAction.ForceUpdate);
                    break;
                case SaveOption.None:
                    _collection.Add(entity, SaveAction.Save);
                    break;
            }
            
        }

        public void Remove(T entity)
        {
            _collection.Add(entity, SaveAction.Remove);
        }

        public bool RemoveLocal(T entity)
        {
            return _collection.Remove(entity);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public List<T> Where(Expression<Func<T, bool>> propertyLambda)
        {
            return _context.Where(propertyLambda);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> propertyLambda)
        {
            return _context.First(propertyLambda);
        }

        public T Find(params object[] pks)
        {
            return _context.Find<T>(pks);
        }

        public List<T> All()
        {
            var builder = new SqlQueryBuilder();
            builder.Table(_tableName);
            builder.SelectAll();

            _context.Execute(builder);

            return _context.All<T>();
        }
    }
}
