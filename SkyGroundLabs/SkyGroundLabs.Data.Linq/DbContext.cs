using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Linq.Mapping;
using SkyGroundLabs.Reflection;
using System.Reflection;

namespace SkyGroundLabs.Data.Linq
{
	public abstract class DbContext : IDisposable
	{
		#region Properties
		private string _connectionString { get; set; }
		private DataContext _context { get; set; }
		#endregion

		#region Constructor
		public DbContext(string connectionString)
		{
			_connectionString = connectionString;
			_context = new DataContext(_connectionString);
			Initialized(_context);
		}

		public DbContext(string server, string database, string userID, string password)
		{
			_connectionString = string.Format(
			"Server={0};Database={1};User Id={2};Password={3};MultipleActiveResultSets=true",
			server,
			database,
			userID,
			password);

			_context = new DataContext(_connectionString);
			Initialized(_context);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Is used to get the contents of a Sql Server Table.
		/// </summary>
		/// <typeparam name="TEntity">Type</typeparam>
		/// <returns>Table</returns>
		public Table<TEntity> GetTable<TEntity, TPKType>()
			where TEntity : DbTableEquatable<IDbTableEquatable<TPKType>>
			where TPKType : struct
		{
			return _context.GetTable<TEntity>();
		}

		protected virtual void Initialized(DataContext context) { }

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
			// check attribute validation first
			_checkAttributesValidation<TEntity, TPKType>(entity);

			var changedList = _context.GetTable<TEntity>();
			var ID = entity.GetType().GetProperty("ID").GetValue(entity);

			_preprocessSave<TEntity, TPKType>(entity);

			// Save changes
			if (Convert.ToInt64(ID) == 0)
			{
				// Insert
				// If No ID we need to insert on submit
				_context.GetTable<TEntity>().InsertOnSubmit((TEntity)entity);
				_context.SubmitChanges();
			}
			else
			{
				// Update
				var item = changedList.Where(w => w.Equals(entity)).FirstOrDefault();
				ReflectionManager.SetValuesWithSkip(entity, item, "ID");
				_context.SubmitChanges();
			}

			Refresh();
		}

		/// <summary>
		/// Saves any non committed changes to the database
		/// </summary>
		public void SaveChanges()
		{
			_context.SubmitChanges();
			Refresh();
		}

		/// <summary>
		/// Checks custom Ion.Data.Linq attributes
		/// </summary>
		/// <typeparam name="TEntity">Record Type</typeparam>
		/// <typeparam name="TPKType">Primary Key Type</typeparam>
		/// <param name="entity">Record</param>
		private void _checkAttributesValidation<TEntity, TPKType>(TEntity entity)
			where TEntity : DbTableEquatable<IDbTableEquatable<TPKType>>
			where TPKType : struct
		{
			var properties = entity.GetType().GetProperties().Where(w => w.GetCustomAttributes<MaxLengthAttribute>().Count() > 0);

			foreach (var property in properties)
			{
				var data = ReflectionManager.GetPropertyValue(entity, property.Name);
				var attribute = property.GetCustomAttribute<MaxLengthAttribute>();

				if (data.ToString().Length > attribute.Length)
				{
					if (attribute.ShouldTruncate)
					{
						ReflectionManager.SetPropertyValue(entity, property.Name, data.ToString().Substring(0, attribute.Length));
					}
					else
					{
						throw new Exception(property.Name + " property length is greater than the max allowed (" + attribute.Length + ")");
					}
				}
			}
		}

		/// <summary>
		/// Marks the record as delete and will be deleted when saved
		/// </summary>
		/// <typeparam name="TEntity">Record Type</typeparam>
		/// <typeparam name="TPKType">Primary Key Type</typeparam>
		/// <param name="entity">Record</param>
		public virtual void DeleteOnSave<TEntity, TPKType>(TEntity entity)
			where TEntity : DbTableEquatable<IDbTableEquatable<TPKType>>
			where TPKType : struct
		{
			var item = _context.GetTable<TEntity>().Where(w => w.Equals(entity)).FirstOrDefault();
			_context.GetTable<TEntity>().DeleteOnSubmit((TEntity)item);
		}

		protected virtual void _preprocessSave<TEntity, TPKType>(TEntity entity)
			where TEntity : DbTableEquatable<IDbTableEquatable<TPKType>>
			where TPKType : struct
		{

		}

		public void Dispose()
		{
			_connectionString = "";
			_context.Dispose();
			_context = null;
		}

		public virtual void Refresh<TEntity>(RefreshMode mode, TEntity entity) where TEntity : class
		{
			_context.Refresh(RefreshMode.OverwriteCurrentValues, entity);
		}

		public virtual void Refresh()
		{
			_context = new DataContext(_connectionString);
		}

		public virtual void Refresh<TEntity>(RefreshMode mode, params TEntity[] entities) where TEntity : class
		{
			_context.Refresh(RefreshMode.OverwriteCurrentValues, entities);
		}

		public virtual void Refresh<TEntity>(RefreshMode mode, IEnumerable<TEntity> entities) where TEntity : class
		{
			_context.Refresh(RefreshMode.OverwriteCurrentValues, entities);
		}
		#endregion
	}
}
