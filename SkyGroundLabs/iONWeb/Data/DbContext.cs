using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Business.Services;
using SkyGroundLabs.Data.Entity;
using SkyGroundLabs.Data.Entity.Mapping;
using iONWeb.Data.Tables;

namespace iONWeb.Data
{
	/// <summary>
	/// For use with custom controls
	/// </summary>
	public class DbContext : DataContext
	{
		#region Tables

		public IDbSet<Company> Companies { get; set; }

		public IDbSet<Customer> Customers { get; set; }

		public IDbSet<Event> Events { get; set; }

		public IDbSet<EventAuthentication> EventAuthentication { get; set; }

		public IDbSet<EventHistory> EventHistory { get; set; }

		public IDbSet<EventReminder> EventReminders { get; set; }

		public IDbSet<User> Users { get; set; }

		public IDbSet<EventInvitee> EventInvitees { get; set; }

		#endregion

		#region Constructor
		public DbContext(string server, string database, string userID, string password)
			: base(server, database, userID, password)
		{
			_initialize();
		}

		public DbContext(string connectionString)
			: base(connectionString)
		{
			_initialize();
		}

		private void _initialize()
		{
		}
		#endregion

		#region Methods
		protected override void _preprocessSave<TEntity, TPKType>(TEntity entity)
		{

		}
		#endregion
	}
}
