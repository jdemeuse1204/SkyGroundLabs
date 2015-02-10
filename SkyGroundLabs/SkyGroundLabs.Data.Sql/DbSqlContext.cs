using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LambdaSqlBuilder;
using SkyGroundLabs.Data.Sql.Commands;
using SkyGroundLabs.Data.Sql.Commands.Support;
using SkyGroundLabs.Data.Sql.Connection;
using SkyGroundLabs.Data.Sql.Mapping;
using SkyGroundLabs.Data.Sql.Mapping.Base;
using SkyGroundLabs.Reflection;


namespace SkyGroundLabs.Data.Sql
{
    public class DbSqlContext : IDisposable
    {
        #region Properties
        protected string _connectionString { get; set; }
        protected SqlConnection _connection { get; set; }
        protected SqlCommand _cmd { get; set; }
        protected SqlDataReader _reader { get; set; }
        public ModificationState ModificationState { get; private set; }
        #endregion

        #region Constructor
        public DbSqlContext(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
        }

        public DbSqlContext(IConnectionBuilder connection)
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

                for (var i = 0; i < rec.FieldCount; i++)
                {
                    keyContainer.Add(rec.GetName(i), rec.GetValue(i));
                }

                _reader.Close();
                _reader.Dispose();

                return keyContainer;
            }

            _reader.Close();
            _reader.Dispose();

            return new KeyContainer();
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

            _reader.Close();
            _reader.Dispose();

            return default(T);
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

            _reader.Close();
            _reader.Dispose();

            return null;
        }

        /// <summary>
        /// Converts an object to a dynamic
        /// </summary>
        /// <returns></returns>
        public dynamic Select()
        {
            return _reader.HasRows ? _reader.ToObject() : null;
        }

        /// <summary>
        /// Converts a datareader to an object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Select<T>()
        {
            return _reader.HasRows ? _reader.ToObject<T>() : default(T);
        }

        /// <summary>
        /// Return list of items
        /// </summary>
        /// <returns>List of type T</returns>
        public List<T> All<T>()
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
        public List<dynamic> All()
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
        public virtual void Delete<T>(T entity)
            where T : class
        {
            // Check to see if the PK is defined
            var tableName = entity.GetDatabaseTableName();

            // ID is the default primary key name
            var primaryKeys = _getPrimaryKeyColumns(entity);

            // delete Data
            var builder = new SqlDeleteBuilder();
            builder.Delete(tableName);

            // Loop through all mapped properties
            foreach (var property in primaryKeys)
            {
                var value = property.GetValue(entity);
                var columnName = property.GetDatabaseColumnName();
                builder.AddWhere(tableName, columnName, ComparisonType.Equals, value);
            }

            // Execute the insert statement
            Execute(builder);

            entity = null;
        }

        /// <summary>
        /// Saves changes to the database.  If there is a PK match values will be updated,
        /// otherwise record will be inserted
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public virtual void SaveChanges<T>(T entity)
            where T : class
        {
            ModificationState = ModificationState.Insert;

            // Check to see if the PK is defined
            var tableName = entity.GetDatabaseTableName();

            // ID is the default primary key name
            var primaryKeys = _getPrimaryKeyColumns(entity);

            // Tells us whether to insert or update
            var isUpdating = false;

            // all table properties
            var tableColumns = entity.GetType().GetProperties().Where(w => w.GetCustomAttribute<UnmappedAttribute>() == null);

            // check to see whether we have an insert or update
            foreach (var pkValue in primaryKeys.Select(primaryKey => primaryKey.GetValue(entity)))
            {
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
                if (!isUpdating)
                {
                    continue;
                }

                ModificationState = ModificationState.Update;
                break;
            }

            // Update Or Insert data
            switch (ModificationState)
            {
                case ModificationState.Update:
                    {
                        // Update Data
                        var update = new SqlUpdateBuilder();
                        update.Table(tableName);

                        foreach (var property in from property in tableColumns let columnName = property.GetDatabaseColumnName() where !primaryKeys.Select(w => w.Name).Contains(property.Name) select property)
                        {
                            // Skip unmapped fields
                            update.AddUpdate(property, entity);
                        }

                        // add validation to only update the row
                        foreach (var primaryKey in primaryKeys)
                        {
                            update.AddWhere(tableName, primaryKey.Name, ComparisonType.Equals, primaryKey.GetValue(entity));
                        }

                        Execute(update);
                    }
                    break;
                case ModificationState.Insert:
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
                        Execute(insert);

                        // set the resulting pk(s) and db generated columns in the entity object
                        foreach (var item in SelectIdentity())
                        {
                            // find the property first in case the column name change attribute is used
                            // Key is property name, value is the db value
                            ReflectionManager.SetPropertyValue(entity, _findPropertyName(entity, item.Key), item.Value);
                        }
                    }
                    break;
            }

            ModificationState = ModificationState.None;
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
            var result = Activator.CreateInstance<T>();

            // get the database table name
            var tableName = result.GetDatabaseTableName();

            var builder = new SqlQueryBuilder();
            builder.SelectAll();
            builder.Table(tableName);

            // Get All PKs
            var keyProperties = _getPrimaryKeyColumns(result);

            for (var i = 0; i < keyProperties.Count(); i++)
            {
                var key = keyProperties[i];

                // check to see if the column is renamed
                var name = key.GetDatabaseColumnName();

                builder.AddWhere(tableName, name, ComparisonType.Equals, pks[i]);
            }

            Execute(builder);

            return First<T>();
        }

        public List<T> Where<T>(Expression<Func<T, bool>> propertyLambda)
            where T : class
        {
            var query = new SqlLam<T>(propertyLambda);
            var builder = new SqlQueryBuilder();
            builder.Select(query.QueryString);

            foreach (var pair in query.QueryParameters)
            {
                builder.AddParameter(string.Format("@{0}", pair.Key), pair.Value);
            }

            // Execute the sql on the db
            Execute(builder);

            return All<T>();
        }

        public T First<T>(Expression<Func<T, bool>> propertyLambda)
            where T : class
        {
            var query = new SqlLam<T>(propertyLambda);
            var builder = new SqlQueryBuilder();
            builder.Select(query.QueryString.ToUpper().Replace("SELECT", "SELECT TOP 1"));

            foreach (var pair in query.QueryParameters)
            {
                builder.AddParameter(string.Format("@{0}", pair.Key), pair.Value);
            }

            // Execute the sql on the db
            Execute(builder);

            return First<T>();
        }

        /// <summary>
        /// Execute the SqlBuilder on the database
        /// </summary>
        /// <param name="builder"></param>
        public void Execute(ISqlBuilder builder)
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
        public void Execute(string sql)
        {
            _cmd = new SqlCommand(sql, _connection);

            _connect();
            _reader = _cmd.ExecuteReader();
        }

        public void Execute<T>(Expression<Func<T, bool>> propertyLambda)
            where T : class
        {
            var query = new SqlLam<T>(propertyLambda);
            var builder = new SqlQueryBuilder();
            builder.Select(query.QueryString);

            foreach (var pair in query.QueryParameters)
            {
                builder.AddParameter(string.Format("@{0}", pair.Key), pair.Value);
            }

            // Execute the sql on the db
            Execute(builder);
        }

        /// <summary>
        /// Used for looping through results
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            if (_reader.Read())
            {
                return true;
            }

            // close reader when no rows left
            _reader.Close();
            _reader.Dispose();
            return false;
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
            var column = properties.FirstOrDefault(w => w.GetCustomAttribute<ColumnAttribute>() != null
                                                        && w.GetCustomAttribute<ColumnAttribute>().Name == lookupName);

            // check for rename first 
            if (column != null)
            {
                return column.Name;
            }

            if (properties.Select(w => w.Name).Contains(lookupName))
            {
                return lookupName;
            }

            throw new Exception("Column Not Found");
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
            var keyList = entity.GetType().GetProperties().Where(w =>
                (w.GetCustomAttribute<SearchablePrimaryKeyAttribute>() != null
                && w.GetCustomAttribute<SearchablePrimaryKeyAttribute>().IsPrimaryKey)
                || (w.Name.ToUpper() == "ID")).ToList();

            if (keyList.Count != 0)
            {
                return keyList;
            }

            throw new Exception("Cannot find PrimaryKey(s)");
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
