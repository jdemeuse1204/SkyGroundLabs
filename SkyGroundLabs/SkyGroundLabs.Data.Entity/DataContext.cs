using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;
using SkyGroundLabs.Reflection;

namespace SkyGroundLabs.Data.Entity
{
	public abstract class DataContext : DbContext
	{
		#region Constructor
		public DataContext(string connectionString)
			: base(connectionString) { }

		public DataContext(string server, string database, string userID, string password)
			: base(string.Format(
						"Server={0};Database={1};User Id={2};Password={3};MultipleActiveResultSets=true",
						server,
						database,
						userID,
						password)) { }
		#endregion

		#region Methods
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			Database.SetInitializer<DataContext>(null);
		}

		/// <summary>
		/// Is used to get the contents of a Sql Server Table.
		/// </summary>
		/// <typeparam name="TEntity">Type</typeparam>
		/// <returns>Table</returns>
		public IDbSet<TEntity> Set<TEntity, TPKType>()
			where TEntity : DbTableEquatable<IDbTableEquatable<TPKType>>
			where TPKType : struct
		{
			return Set<TEntity>();
		}

		/// <summary>
		/// Saves the changes to the database.  In order to save the table must inherit from DbTableEquatable 
		/// and be a type of IDbTableEquatable and the Primary Key Type must be a C# Structure
		/// </summary>
		/// <typeparam name="TEntity">Record Type</typeparam>
		/// <typeparam name="TPKType">Primary Key Type</typeparam>
		/// <param name="entity">Record</param>
		public virtual void SaveChanges<TEntity, TPKType>(TEntity entity)
			where TEntity : DbTableEquatable<IDbTableEquatable<TPKType>>
			where TPKType : struct
		{
			var changedList = Set<TEntity>();
			var ID = entity.GetType().GetProperty("ID").GetValue(entity);
			var defaultValue = Activator.CreateInstance(entity.GetType().GetProperty("ID").PropertyType);

			_preprocessSave<TEntity, TPKType>(entity);
			
			// Save changes
			if (ID.Equals(defaultValue))
			{
				// Insert
				// If No ID we need to insert on submit
				Set<TEntity>().Add((TEntity)entity);
				SaveChanges();
			}
			else
			{
				// Update
				var item = changedList.Find(ID);
				ReflectionManager.SetValuesWithSkip(entity, item, "ID");
				SaveChanges();
			}
		}

		/// <summary>
		/// Marks the record as delete and will be deleted when saved
		/// </summary>
		/// <typeparam name="TEntity">Record Type</typeparam>
		/// <typeparam name="TPKType">Primary Key Type</typeparam>
		/// <param name="entity">Record</param>
		public virtual void Delete<TEntity, TPKType>(TEntity entity)
			where TEntity : DbTableEquatable<IDbTableEquatable<TPKType>>
			where TPKType : struct
		{
			var ID = entity.GetType().GetProperty("ID").GetValue(entity);
			var item = Set<TEntity>().Find(ID);
			Set<TEntity>().Remove((TEntity)item);
			SaveChanges();
		}

		protected virtual void _preprocessSave<TEntity, TPKType>(TEntity entity)
			where TEntity : DbTableEquatable<IDbTableEquatable<TPKType>>
			where TPKType : struct
		{

		}
		#endregion
	}
}
