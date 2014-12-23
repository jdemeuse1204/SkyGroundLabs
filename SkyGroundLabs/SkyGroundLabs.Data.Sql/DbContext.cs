using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Commands;
using SkyGroundLabs.Data.Sql.Commands.Support;
using SkyGroundLabs.Data.Sql.Connection;
using SkyGroundLabs.Data.Sql.Enumeration;
using SkyGroundLabs.Data.Sql.Mapping;
using SkyGroundLabs.Data.Sql.Mapping.Base;
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
		/// <summary>
		/// Used with insert statements only, gets the value if the id's that were inserted
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Converts the first row to type T
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Converts the first row to a dynamic
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Converts an object to a dynamic
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Converts a datareader to an object of type T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
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

		/// <summary>
		/// Return list of items
		/// </summary>
		/// <returns>List of type T</returns>
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

		/// <summary>
		/// Return list of items
		/// </summary>
		/// <returns>List of dynamics</returns>
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
		/// <summary>
		/// Saves changes to the database.  If there is a PK match values will be updated,
		/// otherwise record will be inserted
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		public void SaveChanges<T>(T entity)
			where T : class
		{
			// Check to see if the PK is defined
			var tableName = entity.GetDatabaseTableName();

			// ID is the default primary key name
			var primaryKeys = _getPrimaryKeyColumns(entity);

			// Tells us whether to insert or update
			var isUpdating = false;

			// all table properties
			var tableColumns = entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<UnmappedAttribute>() == null);

			// check to see whether we have an insert or update
			foreach (var primaryKey in primaryKeys)
			{
				var pkValue = primaryKey.GetValue(entity);

				switch (pkValue.GetType().Name.ToUpper())
				{
					case "INT16":
						isUpdating = Convert.ToInt16(pkValue) != 0;
						break;
					case "INT32":
						isUpdating = Convert.ToInt32(pkValue) != 0;
						break;
					case "INT64":
						isUpdating = Convert.ToInt64(pkValue) != 0;
						break;
					case "GUID":
						isUpdating = pkValue != null && (Guid)pkValue != Guid.Empty;
						break;
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

				foreach (var property in tableColumns)
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
				foreach (var property in tableColumns)
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

		/// <summary>
		/// Execute the SqlBuilder on the database
		/// </summary>
		/// <param name="builder"></param>
		public void ExecuteSql(ISqlBuilder builder)
		{
			_cmd = builder.BuildCommand(_connection);

			_connect();
			_reader = _cmd.ExecuteReader();
		}

		/// <summary>
		/// Execute sql statement without sql builder on the database, this should be used for any stored
		/// procedures.  NOTE:  This does not use SqlSecureExecutable to ensure only safe sql strings
		/// are executed
		/// </summary>
		/// <param name="sql"></param>
		public void ExecuteSql(string sql)
		{
			_cmd = new SqlCommand(sql, _connection);

			_connect();
			_reader = _cmd.ExecuteReader();
		}

		/// <summary>
		/// Used for looping through results
		/// </summary>
		/// <returns></returns>
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
			var column = properties.Where(w => w.GetCustomAttribute<ColumnAttribute>() != null
				&& w.GetCustomAttribute<ColumnAttribute>().Name == lookupName).FirstOrDefault();

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

		/// <summary>
		/// Gets all primary key columns
		/// 1.  Where Name equals "ID"    -    
		/// 2.  Where the Column Attribure equals "ID"    -    
		/// 3.  Where the Key Attribute is on a property
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private List<PropertyInfo> _getPrimaryKeyColumns(object entity)
		{
			var pks = new List<PropertyInfo>();
			var keyList = entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<SearchablePrimaryKeyAttribute>() != null
				&& w.GetCustomAttribute<SearchablePrimaryKeyAttribute>().IsPrimaryKey);

			if (keyList != null)
			{
				foreach (var item in keyList)
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
		/// <summary>
		/// Dispose of all connections, readers, and commands
		/// </summary>
		public void Dispose()
		{
			// disconnect our db reader
			_reader.Close();
			_reader.Dispose();

			// dispose of our sql command
			_cmd.Dispose();

			// close our connection
			_connection.Close();
			_connection.Dispose();
		}
		#endregion

		#region Connection Methods
		/// <summary>
		/// Connect our SqlConnection
		/// </summary>
		private void _connect()
		{
			// Open the connection if its closed
			if (_connection.State == ConnectionState.Closed)
			{
				_connection.Open();
			}
		}

		/// <summary>
		/// Disconnect the SqlConnection
		/// </summary>
		public void Disconnect()
		{
			// Disconnect our connection
			_connection.Close();
		}
		#endregion
	}
}
