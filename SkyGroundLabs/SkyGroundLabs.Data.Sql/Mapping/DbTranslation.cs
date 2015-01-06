using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Mapping
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DbTranslationAttribute : Attribute
	{
		// SearchableKeyType needed for quick lookup in iterator
		public DbTranslationAttribute(SqlDbType type) 
		{
			Type = type;
		}

		public SqlDbType Type { get; set; }
	}
}
