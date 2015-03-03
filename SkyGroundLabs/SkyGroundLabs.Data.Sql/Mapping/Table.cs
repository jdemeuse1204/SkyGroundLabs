using System;

namespace SkyGroundLabs.Data.Sql.Mapping
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class TableAttribute : Attribute
	{
		public TableAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}
