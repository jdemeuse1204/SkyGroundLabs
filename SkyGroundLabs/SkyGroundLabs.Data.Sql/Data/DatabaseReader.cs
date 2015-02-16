using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using SkyGroundLabs.Data.Sql.Commands;
using SkyGroundLabs.Data.Sql.Commands.Lambda;
using SkyGroundLabs.Data.Sql.Commands.Support;

namespace SkyGroundLabs.Data.Sql.Data
{
    /// <summary>
    /// All data reading methods in this class require a READ before data can be retreived
    /// </summary>
    public abstract class DatabaseReader : Database
    {
        protected DatabaseReader(string connectionString) 
            : base(connectionString)
        {
        }

        /// <summary>
        /// Used for looping through results
        /// </summary>
        /// <returns></returns>
        public bool Read()
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
        public dynamic Select()
        {
            return Reader.ToObject();
        }

        /// <summary>
        /// Converts a datareader to an object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Select<T>()
        {
            return Reader.ToObject<T>();
        }

        /// <summary>
        /// Execute the SqlBuilder on the database
        /// </summary>
        /// <param name="builder"></param>
        public void Execute(ISqlBuilder builder)
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
        public void Execute(string sql)
        {
            Command = new SqlCommand(sql, Connection);

            Connect();
            Reader = Command.ExecuteReader();
        }

        protected void Execute<T>(Expression<Func<T, bool>> propertyLambda, SqlSelection sqlSelection)
            where T : class
        {
            var resolver = new LambdaResolver();
			var builder = new SqlQueryBuilder();
			var result = resolver.Resolve(propertyLambda, sqlSelection);
			builder.Select(result.QueryString);

			foreach (var pair in result.QueryParameters)
            {
                builder.AddParameter(string.Format("@{0}", pair.Key), pair.Value);
            }

            // Execute the sql on the db
            Execute(builder);
        }
    }
}
