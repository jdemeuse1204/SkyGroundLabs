using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Commands;
using SkyGroundLabs.Data.Sql.Connection;
using SkyGroundLabs.Data.Sql.Enumeration;
using SkyGroundLabs.Data.Sql.Mapping;
using SkyGroundLabs.Data.Sql.Support;
using SkyGroundLabs.Reflection;

namespace SkyGroundLabs.Data.Sql
{
	public class DbContext : IDisposable
	{
		#region Properties
		private SqlConnection _connection { get; set; }
		private SqlCommand _cmd { get; set; }
		private SqlDataReader _reader { get; set; }
		#endregion

		#region Constructor
		public DbContext(string connectionString)
		{
			_connection = new SqlConnection(connectionString);
		}

		public DbContext(IConnectionBuilder connection)
		{
			_connection = new SqlConnection(connection.BuildConnectionString());
		}
		#endregion

		#region Select Methods
		public T First<T>()
		{
			_reader.Read();

			if (_reader.HasRows)
			{
				return _reader.ToObject<T>();
			}
			else
			{
				return default(T);
			}
		}

		protected object SelectIdentity()
		{
			_reader.Read();
			return _reader[0];
		}

		public dynamic First()
		{
			_reader.Read();

			if (_reader.HasRows)
			{
				return _reader.ToObject();
			}
			else
			{
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

			return result;
		}

		public List<dynamic> SelectList()
		{
			var result = new List<dynamic>();

			while (_reader.Read())
			{
				result.Add(_reader.ToObject());
			}

			return result;
		}
		#endregion

		#region Methods
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

		#region Entity Methods
		public void SaveChanges<T>(T entity)
			where T : class
		{
			// Check to see if the PK is defined
			var keyProperties = entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<KeyAttribute>() != null).ToList();
			var tableName = entity.GetType().Name;

			// check for table name attribute
			var tableAttribute = entity.GetType().GetCustomAttribute<TableAttribute>();

			// Set the table name 
			if (tableAttribute != null)
			{
				tableName = tableAttribute.Name;
			}

			if (keyProperties.Count > 0)
			{
				// get all primary keys
				throw new Exception("FINISH YOUR CODE DAMMIT!");
			}
			else
			{
				// ID is the default primary key name
				var keyColumn = entity.GetType().GetProperties().Where(w => w.Name.ToUpper() == "ID").FirstOrDefault();
				var pkNames = new string[] { };
				var pkPropertyNames = new string[] { };

				// if there was no column found look for the column attribute
				if (keyColumn == null)
				{
					keyColumn = entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<ColumnAttribute>().Name == "ID").FirstOrDefault();

					pkNames = new string[] { keyColumn.GetCustomAttribute<ColumnAttribute>().Name };
					pkPropertyNames = new string[] { keyColumn.Name };

					// make sure the PK is named properly
					if (keyColumn == null)
					{
						throw new Exception("Cannot find PrimaryKey ID");
					}
				}
				else
				{
					pkNames = new string[] { keyColumn.Name };
					pkPropertyNames = new string[] { keyColumn.Name };
				}

				// at this point we have the PK Property
				// PK Types, 
				// GUID, INTEGERS

				var pkValue = keyColumn.GetValue(entity);
				var isUpdating = false;

				if (pkValue is Int16 || pkValue is Int32 || pkValue is Int64)
				{
					isUpdating = Convert.ToInt64(pkValue) != 0;
				}
				else if (pkValue is Guid)
				{
					isUpdating = pkValue != null && (Guid)pkValue != Guid.Empty;
				}

				// Update Or Insert
				if (isUpdating)
				{
					// Update Data
					SqlUpdateBuilder update = new SqlUpdateBuilder();
					update.Table(tableName);

					foreach (var property in entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<UnmappedAttribute>() == null))
					{
						var columnName = property.Name;
						var customColumn = property.GetCustomAttribute<ColumnAttribute>();
						var updateValue = property.GetValue(entity);

						if (customColumn != null)
						{
							columnName = customColumn.Name;
						}

						if (pkNames.Contains(columnName))
						{
							// only do this if identity insert is on
							continue;
						}

						// Skip unmapped fields
						update.AddUpdate(columnName, updateValue == null ? "NULL" : updateValue);
					}

					// add validation to only update the row
					update.AddWhere(tableName, pkNames[0], ComparisonType.Equals, pkValue);
					this.ExecuteSql(update);
				}
				else
				{
					// Insert Data
					SqlInsertBuilder insert = new SqlInsertBuilder();
					insert.Table(tableName);

					foreach (var property in entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<UnmappedAttribute>() == null))
					{
						var columnName = property.Name;
						var customColumn = property.GetCustomAttribute<ColumnAttribute>();
						var insertValue = property.GetValue(entity);

						if (customColumn != null)
						{
							columnName = customColumn.Name;
						}

						if (pkNames.Contains(columnName))
						{
							// only do this if identity insert is on
							continue;
						}

						// Skip unmapped fields
						insert.AddInsert(columnName, insertValue == null ? "NULL" : insertValue);
					}

					// Execute the insert statement
					this.ExecuteSql(insert);

					// set the resulting pk in the entity object
					ReflectionManager.SetPropertyValue(entity, pkPropertyNames[0], this.SelectIdentity());
				}
			}
		}

		public T Find<T>(params object[] pks)
		{
			T result = Activator.CreateInstance<T>();
			var tableName = result.GetType().Name;

			// check for table name attribute
			var tableAttribute = result.GetType().GetCustomAttribute<TableAttribute>();

			// Set the table name 
			if (tableAttribute != null)
			{
				tableName = tableAttribute.Name;
			}

			SqlQueryBuilder builder = new SqlQueryBuilder();
			builder.SelectAll();
			builder.Table(tableName);

			// Check to see if the PK is defined
			var keyProperties = result.GetType().GetProperties().Where(w => w.GetCustomAttribute<KeyAttribute>() != null).ToList();

			if (keyProperties.Count > 0)
			{
				for (int i = 0; i < keyProperties.Count; i++)
				{
					var key = keyProperties[i];

					// check to see if the column is renamed
					var columnAttribute = key.GetCustomAttribute<ColumnAttribute>();
					var name = key.Name;

					// if we find a custom name then we need to go grab that name
					if (columnAttribute != null)
					{
						name = columnAttribute.Name;
					}

					builder.AddWhere(tableName, name, ComparisonType.Equals, pks[i]);
				}
			}
			else
			{
				// assume the pk name is ID and we grab the first pk from the array
				builder.AddWhere(tableName, "ID", ComparisonType.Equals, pks[0]);
			}

			this.ExecuteSql(builder);

			return this.First<T>();
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
