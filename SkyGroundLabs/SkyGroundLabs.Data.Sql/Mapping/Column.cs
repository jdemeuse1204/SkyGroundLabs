using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping.Base;

namespace SkyGroundLabs.Data.Sql.Mapping
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class ColumnAttribute : SearchablePrimaryKeyAttribute
	{
		// SearchableKeyType needed for quick lookup in iterator
		public ColumnAttribute(string name) : base(SearchablePrimaryKeyType.Column)
		{
			Name = name;
		}

		public string Name { get; private set; }

		public override bool IsPrimaryKey
		{
			get { return Name.ToUpper() == "ID"; }
		}
	}
}
