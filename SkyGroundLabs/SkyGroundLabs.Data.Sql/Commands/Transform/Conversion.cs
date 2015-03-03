using System.Data;

namespace SkyGroundLabs.Data.Sql.Commands.Transform
{
    public class Conversion
    {
        public static object To(object entity, SqlDbType targetTransformType, int style)
        {
            return entity;
        }
    }
}
