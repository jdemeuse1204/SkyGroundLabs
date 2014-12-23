using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping.Base;

namespace SkyGroundLabs.Data.Sql.Mapping
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class KeyAttribute : SearchablePrimaryKeyAttribute
	{
		// SearchableKeyType needed for quick lookup in iterator
		public KeyAttribute(): base(Enumeration.SearchablePrimaryKeyType.Key) { }

		public override bool IsPrimaryKey
		{
			get { return true; }
		}
	}
}
