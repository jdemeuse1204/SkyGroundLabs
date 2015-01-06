using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Enumeration
{
	public enum ComparisonType
	{
		Contains,
		BeginsWith,
		EndsWith,
		Equals,
		EqualsIgnoreCase,
		EqualsTruncateTime,
		GreaterThan,
		GreaterThanEquals,
		LessThan,
		LessThanEquals,
		NotEqual
	}
}
