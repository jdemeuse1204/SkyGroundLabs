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

		protected KeyContainer SelectIdentity()
		{
			_reader.Read();
			var keyContainer = new KeyContainer();
			var rec = (IDataRecord)_reader;

			for (int i = 0; i < rec.FieldCount; i++)
			{
				keyContainer.Add(rec.GetName(i), rec.GetValue(i));
			}

			return keyContainer;
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

		/// <summary>
		/// Gets the Db Generation Key Type.  If the attribute isnt set returns IdentitySpecification as default
		/// </summary>
		/// <param name="keyColumnProperty"></param>
		/// <returns></returns>
		private DbGenerationType _getDbGenerationOption(PropertyInfo keyColumnProperty)
		{
			var result = DbGenerationType.IdentitySpecification;
			var dbGenerationOptionAttribute = keyColumnProperty.GetCustomAttribute<DbGenerationOptionAttribute>();

			if (dbGenerationOptionAttribute != null)
			{
				result = dbGenerationOptionAttribute.Option;
			}

			return result;
		}

		private string _findPropertyName(object entity, string lookupName)
		{
			var properties = entity.GetType().GetProperties();

			if (properties.Select(w => w.Name).Contains(lookupName))
			{
				return lookupName;
			}

			var column = properties.Where(w => w.GetCustomAttribute<ColumnAttribute>().Name == lookupName).FirstOrDefault();

			if (column != null)
			{
				return column.Name;
			}
			else
			{
				throw new Exception("Column Not Found");
			}
		}

		private string _findPropertyName(object entity, PropertyInfo propertyInfo)
		{
			var columnName = propertyInfo.Name;
			var customColumn = propertyInfo.GetCustomAttribute<ColumnAttribute>();
			var updateValue = propertyInfo.GetValue(entity);

			if (customColumn != null)
			{
				columnName = customColumn.Name;
			}

			return columnName;
		}
		#endregion

		#region Entity Methods
		public void SaveChanges<T>(T entity)
			where T : class
		{
			// Check to see if the PK is defined
			var tableName = entity.GetType().Name;

			// check for table name attribute
			var tableAttribute = entity.GetType().GetCustomAttribute<TableAttribute>();

			// Set the table name 
			if (tableAttribute != null)
			{
				tableName = tableAttribute.Name;
			}

			// ID is the default primary key name
			var primaryKeys = _getPrimaryKeyColumns(entity);
			var isUpdating = false;

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

					if (primaryKeys.Select(w => w.Name).Contains(columnName))
					{
						// only do this if identity insert is on
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

				// find keys we will need to generate
				var keyGenerationColumns = entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<DbGenerationOptionAttribute>() != null
					&& w.GetCustomAttribute<DbGenerationOptionAttribute>().Option == DbGenerationType.Generate).ToList();
				var insert = new SqlInsertBuilder();
				insert.Table(tableName);

				foreach (var property in entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<UnmappedAttribute>() == null))
				{
					var columnName = property.Name;
					var customColumn = property.GetCustomAttribute<ColumnAttribute>();
					var dbGenerationColumn = property.GetCustomAttribute<DbGenerationOptionAttribute>();
					var insertValue = property.GetValue(entity);

					if (customColumn != null)
					{
						columnName = customColumn.Name;
					}

					// is it a primary key
					if (primaryKeys.Select(w => w.Name).Contains(columnName))
					{
						if (!keyGenerationColumns.Select(w => w.Name).Contains(columnName))
						{
							// only do this if identity insert is on
							// only one column allows identity insert per table
							insert.AddIdentity(IdentityType.AtAtIdentity);
							continue;
						}
						else
						{
							insert.AddInsert(columnName, insert.AddIdentity(IdentityType.FromKey));
							continue;
						}
					}

					// Skip unmapped fields
					insert.AddInsert(columnName, insertValue == null ? "NULL" : insertValue);
				}

				// Execute the insert statement
				this.ExecuteSql(insert);

				// set the resulting pk in the entity object
				foreach (var item in this.SelectIdentity())
				{
					// find the property first in case the column name change attribute is used
					ReflectionManager.SetPropertyValue(entity, _findPropertyName(entity, item.Key), item.Value);
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

		#region Insert Methods
		private List<PropertyInfo> _getPrimaryKeyColumns(object entity)
		{
			var pks = new List<PropertyInfo>();
			var keyCheckOne = entity.GetType().GetProperties().Where(w => w.Name.ToUpper() == "ID").FirstOrDefault();
			var keyCheckTwo = entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<ColumnAttribute>() != null && w.GetCustomAttribute<ColumnAttribute>().Name == "ID").FirstOrDefault();
			var keyCheckThree = entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<KeyAttribute>() != null).ToList();

			if (keyCheckOne != null)
			{
				pks.Add(keyCheckOne);
			}

			if (keyCheckTwo != null)
			{
				pks.Add(keyCheckTwo);
			}

			if (keyCheckThree != null)
			{
				pks.AddRange(keyCheckThree);
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
