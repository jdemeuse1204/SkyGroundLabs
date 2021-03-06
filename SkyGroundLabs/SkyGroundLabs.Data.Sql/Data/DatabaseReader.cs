﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using SkyGroundLabs.Data.Sql.Commands.Support;
using SkyGroundLabs.Data.Sql.Expressions;

namespace SkyGroundLabs.Data.Sql.Data
{
    /// <summary>
    /// All data reading methods in this class require a READ before data can be retreived
    /// </summary>
    public abstract class DatabaseReader : Database
    {
        #region Constructor
        protected DatabaseReader(string connectionString) 
            : base(connectionString)
        {
        }
        #endregion

        #region Reader Methods
        /// <summary>
        /// Used for looping through results
        /// </summary>
        /// <returns></returns>
		protected bool Read()
        {
            if (Reader.Read())
            {
                return true;
            }

            // close reader when no rows left
            Reader.Close();
            Reader.Dispose();
            return false;
        }

        /// <summary>
        /// Converts an object to a dynamic
        /// </summary>
        /// <returns></returns>
		protected dynamic Select()
        {
            return Reader.ToObject();
        }

        /// <summary>
        /// Converts a datareader to an object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		protected T Select<T>()
        {
            return Reader.ToObject<T>();
        }
        #endregion

        #region Data Execution
        /// <summary>
        /// Execute the SqlBuilder on the database
        /// </summary>
        /// <param name="builder"></param>
		protected void Execute(ISqlBuilder builder)
        {
            Command = builder.Build(Connection);

            Connect();
            Reader = Command.ExecuteReader();
        }

        /// <summary>
        /// Execute sql statement without sql builder on the database, this should be used for any stored
        /// procedures.  NOTE:  This does not use SqlSecureExecutable to ensure only safe sql strings
        /// are executed
        /// </summary>
        /// <param name="sql"></param>
		protected void Execute(string sql)
        {
            Command = new SqlCommand(sql, Connection);

            Connect();
            Reader = Command.ExecuteReader();
        }

        protected void Execute(string sql, Dictionary<string, object> parameters)
        {
            Command = new SqlCommand(sql, Connection);

            foreach (var item in parameters)
            {
                Command.Parameters.Add(Command.CreateParameter()).ParameterName = item.Key;
                Command.Parameters[item.Key].Value = item.Value;
            }

            Connect();
            Reader = Command.ExecuteReader();
        }

        protected ExpressionQuery Execute<T>(Expression<Func<T, bool>> propertyLambda, DataFetching fetching)
            where T : class
        {
            return new ExpressionQuery(DatabaseSchemata.GetTableName<T>(), fetching);
        }
        #endregion

        #region Query Execution
        public DataReader<T> ExecuteQuery<T>(string sql)
		{
			Execute(sql);

			return new DataReader<T>(Reader);
		}

        public DataReader<T> ExecuteQuery<T>(string sql, Dictionary<string,object> parameters)
        {
            Execute(sql, parameters);

            return new DataReader<T>(Reader);
        }

        public DataReader<T> ExecuteQuery<T>(ISqlBuilder builder)
        {
            Execute(builder);

            return new DataReader<T>(Reader);
        }
        #endregion
    }
}
