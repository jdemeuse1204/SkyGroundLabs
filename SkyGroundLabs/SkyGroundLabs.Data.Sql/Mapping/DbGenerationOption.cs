using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping.Base;

namespace SkyGroundLabs.Data.Sql.Mapping
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DbGenerationOptionAttribute : Attribute
	{
        public DbGenerationOptionAttribute(DbGenerationOption option)
		{
			Option = option;
		}

        public DbGenerationOption Option { get; private set; }
	}

    public enum DbGenerationOption
    {
        None,
        IdentitySpecification,
        Generate
    }
}
