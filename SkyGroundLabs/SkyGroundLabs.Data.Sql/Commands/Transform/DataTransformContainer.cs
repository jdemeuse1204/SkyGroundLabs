using System.Data;

namespace SkyGroundLabs.Data.Sql.Commands.Transform
{
    public sealed class DataTransformContainer
    {
        public DataTransformContainer(object value, SqlDbType transformType)
        {
            Value = value;
            Transform = transformType;
        }

        public object Value { get; private set; }

        public SqlDbType Transform { get; private set; }
    }
}
