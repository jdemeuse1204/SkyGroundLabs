using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Linq.Mapping
{
	/// <summary>
	/// This Class Assumes the type T has a property called ID. MUST be 
	/// used with IDbEquatable
	/// </summary>
	/// <typeparam name="T">Class (Table) that Inherits from IDbEquatable</typeparam>
	public abstract class DbTableEquatable<T> : IEquatable<T> where T : class
	{
		public bool Equals(T other)
		{
			//Check whether the compared object is null.  
			if (Object.ReferenceEquals(other, null))
			{
				return false;
			}

			//Check whether the compared object references the same data.  
			if (Object.ReferenceEquals(this, other))
			{
				return true;
			}

			return ((dynamic)other).ID == ((dynamic)this).ID;
		}
	}
}
