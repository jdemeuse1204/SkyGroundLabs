using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Connection;

namespace SkyGroundLabs.Data.Sql.Entity
{
	public abstract class DbEntityContext : IDisposable
	{
		// do not want to expose sql context's methods
		private readonly DbSqlContext _context;

		public DbEntityContext(string connectionString)
		{
			_context = new DbSqlContext(connectionString);
            _instantiateTables();
		}

		public DbEntityContext(IConnectionBuilder connection)
		{
			_context = new DbSqlContext(connection);
            _instantiateTables();
		}

		private void _instantiateTables()
		{
		    var properties = this.GetType().GetProperties()
		        .Where(w => w.PropertyType.IsInterface 
                    && w.PropertyType.GetGenericTypeDefinition() == typeof (IDbTable<>));

            foreach (var property in properties)
            {
                var generic = property.PropertyType.GetGenericArguments()[0];
                var dbTable = typeof(DbTable<>);
                var creationType = dbTable.MakeGenericType(generic);
                var instance = Activator.CreateInstance(creationType, _context );
                property.SetValue(this, instance, null);
			}
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
