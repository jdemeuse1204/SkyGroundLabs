using System;
using SkyGroundLabs.Data.Sql.Mapping.Base;

namespace SkyGroundLabs.Data.Sql.Mapping
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class KeyAttribute : SearchablePrimaryKeyAttribute
	{
		// SearchableKeyType needed for quick lookup in iterator
		public KeyAttribute(): base(SearchablePrimaryKeyType.Key) { }

		public override bool IsPrimaryKey
		{
			get { return true; }
		}
	}
}
