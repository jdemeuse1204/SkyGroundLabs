using System.Collections.Generic;
using System.Data.SqlClient;
using SkyGroundLabs.Data.Sql.Commands.Support;

namespace SkyGroundLabs.Data.Sql.Commands
{
    public class SqlDeleteBuilder : SqlValidation, ISqlBuilder
    {
        #region Properties
        private string _delete { get; set; }
        private string _table { get; set; }
        private Dictionary<string, object> _parameters { get; set; }
        #endregion

        #region Constructor
        public SqlDeleteBuilder()
        {
            _delete = string.Empty;
            _table = string.Empty;
            _parameters = new Dictionary<string, object>();
        }
        #endregion

        #region Methods
        public SqlCommand BuildCommand(SqlConnection connection)
        {
            if (string.IsNullOrWhiteSpace(_delete))
            {
                throw new QueryNotValidException("DELETE statement missing");
            }

            var sql = _delete + GetValidation();
            var cmd = new SqlCommand(sql, connection);

            InsertParameters(cmd);

            return cmd;
        }

        public void Table(string tableName)
        {
            _table = tableName;
        }

        public void Delete(string tableName)
        {
            _table = tableName;
            _delete = string.Format(" DELETE FROM {0} ", tableName);
        }
        #endregion
    }
}
