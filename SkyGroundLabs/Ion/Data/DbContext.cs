﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ion.Business.Services;
using SkyGroundLabs.Data.Entity;
using SkyGroundLabs.Data.Entity.Mapping;
using Ion.Data.Methods;
using Ion.Data.Tables;
using Ion.Security;

namespace Ion.Data
{
	/// <summary>
	/// For use with custom controls
	/// </summary>
	public class DbContext : DataContext
	{
		#region Tables
		public IDbSet<Appointment> Appointments { get; set; }

		public IDbSet<Contact> Contacts { get; set; }

		public IDbSet<Floor> Floors { get; set; }

		public IDbSet<Indicator> Indicators { get; set; }

		public IDbSet<MoveType> MoveTypes { get; set; }

		public IDbSet<Parameter> Parameters { get; set; }

		public IDbSet<Store> Stores { get; set; }

		public IDbSet<User> Users { get; set; }

		public IDbSet<AppointmentContentsView> ContentsView { get; set; }

		public IDbSet<AppointmentDisplayTitle> DisplayTitle { get; set; }

		public IDbSet<Crew> Crews { get; set; }

		public IDbSet<EmailAccount> EmailAccounts { get; set; }

		public IDbSet<EmailRecent> EmailRecents { get; set; }

		public IDbSet<Safe> Safes { get; set; }

		public IDbSet<ContactArchive> ContactsArchive { get; set; }

		public IDbSet<CrewAssignment> CrewAssignments { get; set; }

		public IDbSet<UserClocking> UserClockings { get; set; }

		public IDbSet<UserRoles> UserRoles { get; set; }

		public IDbSet<UserRoleAccess> UserRoleAccess { get; set; }

		public IDbSet<UserRoleAccessPages> UserRoleAccessPages { get; set; }

		public IDbSet<UserClockingSpecialCode> UserClockingSpecialCodes { get; set; }

		public IDbSet<AppInfo> ApplicationInformation { get; set; }
		#endregion

		#region Functions
		public ContactMethods ContactFunctions { get; set; }

		public AppointmentMethods AppointmentFunctions { get; set; }

		public UserMethods UserFunctions { get; set; }

		public ClockingMethods ClockingFunctions { get; set; }

		public Authentication AuthenticationFunctions { get; set; }
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
			ContactFunctions = new ContactMethods(this);
			AppointmentFunctions = new AppointmentMethods(this);
			UserFunctions = new UserMethods(this);
			ClockingFunctions = new ClockingMethods(this);
			AuthenticationFunctions = new Authentication(this);
		}
		#endregion

		#region Methods
		public virtual void Delete<TEntity>(TEntity entity)
			where TEntity : DbTableEquatable<IDbTableEquatable<long>>
		{
			base.Delete<TEntity, long>(entity);
		}

		public virtual void SaveChanges<TEntity>(TEntity entity)
			where TEntity : DbTableEquatable<IDbTableEquatable<long>>
		{
			base.SaveChanges<TEntity, long>(entity);
		}

		public IDbSet<TEntity> GetTable<TEntity>()
			where TEntity : DbTableEquatable<IDbTableEquatable<long>>
		{
			return base.Set<TEntity, long>();
		}

		protected override void _preprocessSave<TEntity, TPKType>(TEntity entity)
		{
			if (entity is Store)
			{
				var store = ((dynamic)entity);
				store.DisplayName = store.Name + " - " + store.City;

				// default string checks
				if (string.IsNullOrWhiteSpace(store.Zip))
				{
					store.Zip = "";
				}
			}
			else if (entity is Contact)
			{
				var contact = ((dynamic)entity);
				contact.DisplayAddress = ContactServices.BuildAddress(
							contact.Street,
							contact.City,
							contact.State,
							contact.Zip);

				contact.DisplayName1 = ContactServices.BuildDisplayName(
							contact.FirstName1,
							contact.LastName1);

				contact.DisplayName2 = ContactServices.BuildDisplayName(
							contact.FirstName2,
							contact.LastName2);

				contact.DisplayName3 = ContactServices.BuildDisplayName(
							contact.FirstName3,
							contact.LastName3);

				// default string checks
				if (string.IsNullOrWhiteSpace(contact.Zip))
				{
					contact.Zip = "";
				}

				contact.DateEdited = DateTime.Now;
			}
			else if (entity is Appointment)
			{
				var appointment = ((dynamic)entity);
				appointment.DateEdited = DateTime.Now;

				if (string.IsNullOrWhiteSpace(appointment.DropoffZip))
				{
					appointment.DropoffZip = "";
				}

				if (string.IsNullOrWhiteSpace(appointment.PickupZip))
				{
					appointment.PickupZip = "";
				}
			}
		}
		#endregion
	}
}
