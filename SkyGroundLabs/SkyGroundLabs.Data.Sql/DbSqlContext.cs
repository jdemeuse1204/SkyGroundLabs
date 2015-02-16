using SkyGroundLabs.Data.Sql.Connection;
using SkyGroundLabs.Data.Sql.Data;

namespace SkyGroundLabs.Data.Sql
{
    public class DbSqlContext : DataOperations
    {
        #region Constructor
        public DbSqlContext(string connectionString)
            : base(connectionString) { }

        public DbSqlContext(IConnectionBuilder connection)
            : base(connection.BuildConnectionString()) { }
        #endregion
    }
}
