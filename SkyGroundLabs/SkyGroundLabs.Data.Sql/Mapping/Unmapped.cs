using System;

namespace SkyGroundLabs.Data.Sql.Mapping
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class UnmappedAttribute : Attribute
    {

    }
}
