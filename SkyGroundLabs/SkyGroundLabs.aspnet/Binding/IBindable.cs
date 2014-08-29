using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.aspnet.Binding
{
	public interface IBindable
	{
		string Path { get; set; }
		string PropertyName { get; set; }
	}
}
