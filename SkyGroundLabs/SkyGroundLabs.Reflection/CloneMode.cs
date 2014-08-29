using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Reflection
{
	/// <summary>
	/// Skip will skip properties in the list.
	/// Include will only include the properties in the list.
	/// Default will set all properties
	/// </summary>
	public enum CloneMode
	{
		Skip,
		Include,
		Default
	}
}
