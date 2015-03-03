using System;
using System.Data;
using SkyGroundLabs.Data.Sql.Commands;

namespace SkyGroundLabs.Data.Sql.Expressions.Resolver
{
    public sealed class ExpressionWhereResult
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public string TableName { get; set; }

        private object _compareValue;
        public object CompareValue
        {
            get { return _compareValue; }
            set { _compareValue = (value ?? DBNull.Value); }
        }

        public SqlDbType Transform { get; set; }

        public ComparisonType ComparisonType { get; set; }

        public bool ShouldCast { get; set; }
    }
}
