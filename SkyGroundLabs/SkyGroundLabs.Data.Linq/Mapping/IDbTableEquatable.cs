using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Linq.Mapping
{
	/// <summary>
	/// Needs to be inherited from in order for Ion.Data.Linq functions to work
	/// </summary>
	/// <typeparam name="T">Primary Key Type</typeparam>
	public interface IDbTableEquatable<T>
	{
		T ID { get; set; }
	}
}
