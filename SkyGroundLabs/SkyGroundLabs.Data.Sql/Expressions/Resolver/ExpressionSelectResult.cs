using System;
using System.Data;

namespace SkyGroundLabs.Data.Sql.Expressions.Resolver
{
    public sealed class ExpressionSelectResult
    {
        public string ColumnName { get; set; }

        public Type ColumnType { get; set; }

        public string TableName { get; set; }

        public SqlDbType Transform { get; set; }

        public bool ShouldCast { get; set; }
    }
}
