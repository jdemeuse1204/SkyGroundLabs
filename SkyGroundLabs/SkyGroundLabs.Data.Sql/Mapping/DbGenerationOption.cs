using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Enumeration;
using SkyGroundLabs.Data.Sql.Mapping.Base;

namespace SkyGroundLabs.Data.Sql.Mapping
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DbGenerationOptionAttribute : Attribute
	{
		public DbGenerationOptionAttribute(DbGenerationType option)
		{
			Option = option;
		}

		public DbGenerationType Option { get; private set; }
	}
}
