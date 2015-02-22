using System;
using SkyGroundLabs.Data.Sql.Commands;

namespace SkyGroundLabs.Data.Sql.Expressions
{
    public class ExpressionWhereResult
    {
        public string PropertyName { get; set; }
        public string TableName { get; set; }

        private object _compareValue;
        public object CompareValue
        {
            get { return _compareValue; }
            set { _compareValue = (value ?? DBNull.Value); }
        }

        public ComparisonType ComparisonType { get; set; }
    }
}
