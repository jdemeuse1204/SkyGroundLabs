using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Connection;

namespace SkyGroundLabs.Data.Sql
{
	public abstract class DbEntityContext : IDisposable
	{
		// do not want to expose sql context's methods
		private DbSqlContext _context;

		public DbEntityContext(string connectionString)
		{
			_context = new DbSqlContext(connectionString);
			_start();
		}

		public DbEntityContext(SqlServerConnectionBuilder connection)
		{
			_context = new DbSqlContext(connection);
			_start();
		}

		private void _start()
		{
			foreach (var property in this.GetType().GetProperties(BindingFlags.Public).Where(w => w.PropertyType == DBTable)
			{
				var instance = Activator.CreateInstance(property.PropertyType);
				property.SetValue(this, instance, null);
			}
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
