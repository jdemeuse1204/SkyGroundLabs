﻿using System;
using System.Data;
using System.Data.SqlClient;
using SkyGroundLabs.Data.Sql.Expressions;
using SkyGroundLabs.Data.Sql.Expressions.Resolver;

namespace SkyGroundLabs.Data.Sql.Data
{
    public abstract class Database : ExpressionResolver, IDisposable
    {
        protected string ConnectionString { get; private set; }
        protected SqlConnection Connection { get; set; }
        protected SqlCommand Command { get; set; }
        protected SqlDataReader Reader { get; set; }

        protected Database(string connectionString)
        {
            ConnectionString = connectionString;
            Connection = new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Connect our SqlConnection
        /// </summary>
        protected void Connect()
        {
            // Open the connection if its closed
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }

        /// <summary>
        /// Disconnect the SqlConnection
        /// </summary>
        public void Disconnect()
        {
            // Disconnect our connection
            Connection.Close();
        }

        public void Dispose()
        {
            // disconnect our db reader
            Reader.Close();
            Reader.Dispose();

            // dispose of our sql command
            Command.Dispose();

            // close our connection
            Connection.Close();
            Connection.Dispose();
        }
    }
}
