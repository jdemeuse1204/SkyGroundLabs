﻿using System;
using System.Linq;
using SkyGroundLabs.Data.Sql.Commands;
using SkyGroundLabs.Reflection;

namespace SkyGroundLabs.Data.Sql.Data
{
    /// <summary>
    /// This class uses all data fetching functions to create their own functions
    /// </summary>
    public abstract class DataOperations : DataFetching
    {
        #region Constructor
        protected DataOperations(string connectionString) 
            : base(connectionString)
        {
        }
        #endregion

        #region Methods
        public virtual bool Delete<T>(T entity)
            where T : class
        {
            // Check to see if the PK is defined
            var tableName = DatabaseSchemata.GetTableName(entity);

            // ID is the default primary key name
            var primaryKeys = DatabaseSchemata.GetPrimaryKeys(entity);

            // delete Data
            var builder = new SqlDeleteBuilder();
            builder.Delete(tableName);

            // Loop through all mapped properties
            foreach (var property in primaryKeys)
            {
                var value = property.GetValue(entity);
                var columnName = DatabaseSchemata.GetColumnName(property);
                builder.AddWhere(tableName, columnName, ComparisonType.Equals, value);
            }

            // Execute the insert statement
            Execute(builder);

            if (!Reader.HasRows) return false;

            Read();

            var rowsAffected = Reader.GetInt32(0);

            // close our readers
            Reader.Close();
            Reader.Dispose();

            // return if anything was deleted
            return rowsAffected > 0;
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
            // Check to see if the PK is defined
            var tableName = DatabaseSchemata.GetTableName(entity);

            // ID is the default primary key name
            var primaryKeys = DatabaseSchemata.GetPrimaryKeys(entity);

            // all table properties
            var tableColumns = DatabaseSchemata.GetTableFields(entity);

            // check to see whether we have an insert or update
            var state = DatabaseEntity.GetState(entity,primaryKeys);

            // Update Or Insert data
            switch (state)
            {
                case ModificationState.Update:
                    {
                        // Update Data
                        var update = new SqlUpdateBuilder();
                        update.Table(tableName);

                        foreach (var property in from property in tableColumns let columnName = DatabaseSchemata.GetColumnName(property) where !primaryKeys.Select(w => w.Name).Contains(property.Name) select property)
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
                            ReflectionManager.SetPropertyValue(
                                entity,
                                item.Key,
                                item.Value);
                        }
                    }
                    break;
            }

            // close our readers
            Reader.Close();
            Reader.Dispose();
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
            var tableName = DatabaseSchemata.GetTableName(result);

            var builder = new SqlQueryBuilder();
            builder.SelectAll();
            builder.Table(tableName);

            // Get All PKs
            var keyProperties = DatabaseSchemata.GetPrimaryKeys(result);

            for (var i = 0; i < keyProperties.Count(); i++)
            {
                var key = keyProperties[i];

                // check to see if the column is renamed
                var name = DatabaseSchemata.GetColumnName(key);

                builder.AddWhere(tableName, name, ComparisonType.Equals, pks[i]);
            }

            Execute(builder);

            return First<T>();
        }
        #endregion
    }
}
