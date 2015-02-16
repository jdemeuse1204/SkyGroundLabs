using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using SkyGroundLabs.Data.Sql.Commands;
using SkyGroundLabs.Data.Sql.Commands.Support;

namespace SkyGroundLabs.Data.Sql.Data
{
    /// <summary>
    /// All data reading methods in this class do not require a READ before data can be retreived
    /// </summary>
    public abstract class DataFetching : DatabaseReader
    {
        protected DataFetching(string connectionString) 
            : base(connectionString)
        {
        }

        /// <summary>
        /// Used with insert statements only, gets the value if the id's that were inserted
        /// </summary>
        /// <returns></returns>
        protected KeyContainer SelectIdentity()
        {
            if (Reader.HasRows)
            {
                Reader.Read();
                var keyContainer = new KeyContainer();
                var rec = (IDataRecord)Reader;

                for (var i = 0; i < rec.FieldCount; i++)
                {
                    keyContainer.Add(rec.GetName(i), rec.GetValue(i));
                }

                Reader.Close();
                Reader.Dispose();

                return keyContainer;
            }

            Reader.Close();
            Reader.Dispose();

            return new KeyContainer();
        }

        public T First<T>(Expression<Func<T, bool>> propertyLambda)
            where T : class
        {
			Execute(propertyLambda, SqlSelection.Top_1);

            return First<T>();
        }

        /// <summary>
        /// Converts the first row to type T
        /// </summary>
        /// <returns></returns>
        public T First<T>()
        {
            Reader.Read();

            if (Reader.HasRows)
            {
                var result = Reader.ToObject<T>();

                Reader.Close();
                Reader.Dispose();

                return result;
            }

            Reader.Close();
            Reader.Dispose();

            return default(T);
        }

        /// <summary>
        /// Converts the first row to a dynamic
        /// </summary>
        /// <returns></returns>
        public dynamic First(SqlQueryBuilder builder)
        {
            Execute(builder);

            Reader.Read();

            if (Reader.HasRows)
            {
                var result = Reader.ToObject();

                Reader.Close();
                Reader.Dispose();

                return result;
            }

            Reader.Close();
            Reader.Dispose();

            return null;
        }

        /// <summary>
        /// Return list of items
        /// </summary>
        /// <returns>List of type T</returns>
        public List<T> All<T>()
        {
            var builder = new SqlQueryBuilder();
            builder.SelectAll<T>();

            Execute(builder);

            var result = new List<T>();

            while (Reader.Read())
            {
                result.Add(Reader.ToObject<T>());
            }

            Reader.Close();
            Reader.Dispose();

            return result;
        }

		public List<T> All<T>(string sql)
		{
			Execute(sql);

			var result = new List<T>();

			while (Reader.Read())
			{
				result.Add(Reader.ToObject<T>());
			}

			Reader.Close();
			Reader.Dispose();

			return result;
		}

        /// <summary>
        /// Return list of items
        /// </summary>
        /// <returns>List of dynamics</returns>
        public List<dynamic> All(SqlQueryBuilder builder)
        {
            var result = new List<dynamic>();

            Execute(builder);

            while (Reader.Read())
            {
                result.Add(Reader.ToObject());
            }

            Reader.Close();
            Reader.Dispose();

            return result;
        }
    }
}
