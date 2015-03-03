using System.Data;

namespace SkyGroundLabs.Data.Sql.Commands.Transform
{
    public class Cast
    {
        public static object As(object entity, SqlDbType targetTransformType)
        {
            return entity;
        }
    }
}
