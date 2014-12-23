using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Enumeration;

namespace SkyGroundLabs.Data.Sql.Mapping.Base
{
	/// <summary>
	/// All Sql Attribute types must inherit from this class. This creates a way to 
	/// lookup all attributes instead of searching for them individually.
	/// </summary>
	public abstract class SearchablePrimaryKeyAttribute : Attribute
	{
		protected SearchablePrimaryKeyAttribute(SearchablePrimaryKeyType searchableKeyType) 
		{
			SearchableKeyType = searchableKeyType;
		}

		public SearchablePrimaryKeyType SearchableKeyType { get; private set; }

		public abstract bool IsPrimaryKey { get; }
	}
}
