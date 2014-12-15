using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Commands;
using SkyGroundLabs.Data.Sql.Connection;
using SkyGroundLabs.Data.Sql.Enumeration;
using SkyGroundLabs.Data.Sql.KeyGeneration;
using SkyGroundLabs.Data.Sql.Mapping;
using SkyGroundLabs.Data.Sql.Support;
using SkyGroundLabs.Reflection;

namespace SkyGroundLabs.Data.Sql
{
	public class DbContext : IDisposable
	{
		#region Properties
		private string _connectionString { get; set; }
		private SqlConnection _connection { get; set; }
		private SqlCommand _cmd { get; set; }
		private SqlDataReader _reader { get; set; }
		#endregion

		#region Constructor
		public DbContext(string connectionString)
		{
			_connectionString = connectionString;
			_connection = new SqlConnection(_connectionString);
		}

		public DbContext(IConnectionBuilder connection)
		{
			_connectionString = connection.BuildConnectionString();
			_connection = new SqlConnection(_connectionString);
		}
		#endregion

		#region Select/Read Methods
		public T First<T>()
		{
			_reader.Read();

			if (_reader.HasRows)
			{
				var result = _reader.ToObject<T>();

				_reader.Close();
				_reader.Dispose();

				return result;
			}
			else
			{
				_reader.Close();
				_reader.Dispose();

				return default(T);
			}
		}

		protected KeyContainer SelectIdentity()
		{
			if (_reader.HasRows)
			{
				_reader.Read();
				var keyContainer = new KeyContainer();
				var rec = (IDataRecord)_reader;

				for (int i = 0; i < rec.FieldCount; i++)
				{
					keyContainer.Add(rec.GetName(i), rec.GetValue(i));
				}

				_reader.Close();
				_reader.Dispose();

				return keyContainer;
			}
			else
			{
				_reader.Close();
				_reader.Dispose();

				return new KeyContainer();
			}
		}

		public dynamic First()
		{
			_reader.Read();

			if (_reader.HasRows)
			{
				dynamic result = _reader.ToObject();

				_reader.Close();
				_reader.Dispose();

				return result;
			}
			else
			{
				_reader.Close();
				_reader.Dispose();

				return null;
			}
		}

		public dynamic Select()
		{
			if (_reader.HasRows)
			{
				return _reader.ToObject();
			}
			else
			{
				return null;
			}
		}

		public T Select<T>()
		{
			if (_reader.HasRows)
			{
				return _reader.ToObject<T>();
			}
			else
			{
				return default(T);
			}
		}

		public List<T> SelectList<T>()
		{
			var result = new List<T>();

			while (_reader.Read())
			{
				result.Add(_reader.ToObject<T>());
			}

			_reader.Close();
			_reader.Dispose();

			return result;
		}

		public List<dynamic> SelectList()
		{
			var result = new List<dynamic>();

			while (_reader.Read())
			{
				result.Add(_reader.ToObject());
			}

			_reader.Close();
			_reader.Dispose();

			return result;
		}
		#endregion

		#region Entity Methods
		public void SaveChanges<T>(T entity)
			where T : class
		{
			// Check to see if the PK is defined
			var tableName = entity.GetDatabaseTableName();

			// ID is the default primary key name
			var primaryKeys = _getPrimaryKeyColumns(entity);

			// Tells us whether to insert or update
			var isUpdating = false;

			// check to see whether we have an insert or update
			foreach (var primaryKey in primaryKeys)
			{
				var pkValue = primaryKey.GetValue(entity);

				if (pkValue is Int16 || pkValue is Int32 || pkValue is Int64)
				{
					isUpdating = Convert.ToInt64(pkValue) != 0;
				}
				else if (pkValue is Guid)
				{
					isUpdating = pkValue != null && (Guid)pkValue != Guid.Empty;
				}

				// break because we are already updating, do not want to set to false
				if (isUpdating)
				{
					break;
				}
			}

			// Update Or Insert data
			if (isUpdating)
			{
				// Update Data
				SqlUpdateBuilder update = new SqlUpdateBuilder();
				update.Table(tableName);

				foreach (var property in entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<UnmappedAttribute>() == null))
				{
					var columnName = property.GetDatabaseColumnName();
					var updateValue = property.GetValue(entity);

					// DO NOT UPDATE PRIMARY KEYS NO MATTER WHAT
					if (primaryKeys.Select(w => w.Name).Contains(property.Name))
					{
						continue;
					}

					// Skip unmapped fields
					update.AddUpdate(columnName, updateValue == null ? "NULL" : updateValue);
				}

				// add validation to only update the row
				foreach (var primaryKey in primaryKeys)
				{
					update.AddWhere(tableName, primaryKey.Name, ComparisonType.Equals, primaryKey.GetValue(entity));
				}

				this.ExecuteSql(update);
			}
			else
			{
				// Insert Data
				var insert = new SqlInsertBuilder();
				insert.Table(tableName);

				// Loop through all mapped properties
				foreach (var property in entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<UnmappedAttribute>() == null))
				{
					insert.AddInsert(property, entity);
				}

				// Execute the insert statement
				this.ExecuteSql(insert);

				// set the resulting pk(s) and db generated columns in the entity object
				foreach (var item in this.SelectIdentity())
				{
					// find the property first in case the column name change attribute is used
					// Key is property name, value is the db value
					ReflectionManager.SetPropertyValue(entity, _findPropertyName(entity, item.Key), item.Value);
				}
			}
		}

		/// <summary>
		/// Finds a data object by looking for PK matches
		/// </summary>
		/// <typeparam name="T">Must be a Class</typeparam>
		/// <param name="pks"></param>
		/// <returns>Class Object</returns>
		public T Find<T>(params object[] pks)
			where T : class
		{
			T result = Activator.CreateInstance<T>();

			// get the database table name
			var tableName = result.GetDatabaseTableName();

			SqlQueryBuilder builder = new SqlQueryBuilder();
			builder.SelectAll();
			builder.Table(tableName);

			// Get All PKs
			var keyProperties = _getPrimaryKeyColumns(result);

			if (keyProperties.Count > 0)
			{
				for (int i = 0; i < keyProperties.Count; i++)
				{
					var key = keyProperties[i];

					// check to see if the column is renamed
					var name = key.GetDatabaseColumnName();

					builder.AddWhere(tableName, name, ComparisonType.Equals, pks[i]);
				}
			}
			else
			{
				throw new Exception("Primary Key Not Defined");
			}

			this.ExecuteSql(builder);

			return this.First<T>();
		}

		public void ExecuteSql(ISqlBuilder builder)
		{
			_cmd = builder.BuildCommand(_connection);

			_connect();
			_reader = _cmd.ExecuteReader();
		}

		/// <summary>
		/// Execute sql statement without sql builder on the database
		/// </summary>
		/// <param name="sql"></param>
		public void ExecuteSql(string sql)
		{
			_cmd = new SqlCommand(sql, _connection);

			_connect();
			_reader = _cmd.ExecuteReader();
		}

		public bool HasNext()
		{
			return _reader.Read();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Method to look up the name in case the column property was used to rename it
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="lookupName"></param>
		/// <returns></returns>
		private string _findPropertyName(object entity, string lookupName)
		{
			var properties = entity.GetType().GetProperties();
			var column = properties.Where(w => w.GetCustomAttribute<ColumnAttribute>().Name == lookupName).FirstOrDefault();

			// check for rename first 
			if (column != null)
			{
				return column.Name;
			}

			if (properties.Select(w => w.Name).Contains(lookupName))
			{
				return lookupName;
			}
			else
			{
				throw new Exception("Column Not Found");
			}
		}

		private List<PropertyInfo> _getPrimaryKeyColumns(object entity)
		{
			var pks = new List<PropertyInfo>();
			var keyCheckOne = entity.GetType().GetProperties().Where(w => w.Name.ToUpper() == "ID").FirstOrDefault();
			var keyCheckTwo = entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<ColumnAttribute>() != null && w.GetCustomAttribute<ColumnAttribute>().Name == "ID").FirstOrDefault();
			var keyCheckThree = entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<KeyAttribute>() != null).ToList();

			if (keyCheckOne != null && !pks.Select(w => w.Name).Contains(keyCheckOne.Name))
			{
				pks.Add(keyCheckOne);
			}

			if (keyCheckTwo != null && !pks.Select(w => w.Name).Contains(keyCheckTwo.Name))
			{
				pks.Add(keyCheckTwo);
			}

			if (keyCheckThree != null)
			{
				foreach (var item in keyCheckThree)
				{
					if (!pks.Select(w => w.Name).Contains(item.Name))
					{
						pks.Add(item);
					}
				}
			}

			if (pks.Count == 0)
			{
				throw new Exception("Cannot find PrimaryKey(s)");
			}

			return pks;
		}

		#endregion

		#region Dispose
		public void Dispose()
		{
			_cmd.Dispose();
			_connection.Close();
			_connection.Dispose();
		}
		#endregion

		#region Connection Methods
		private void _connect()
		{
			if (_connection.State == ConnectionState.Closed)
			{
				_connection.Open();
			}
		}

		public void Disconnect()
		{
			_connection.Close();
		}
		#endregion
	}
}
