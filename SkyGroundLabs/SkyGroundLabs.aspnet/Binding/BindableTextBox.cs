using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace SkyGroundLabs.aspnet.Binding
{
	public class BindableTextBox : TextBox, IBindable
	{
		public string Path { get; set; }
		public string PropertyName { get; set; }
		public PushConverter PushConversion { get; set; }
		public PullConverter PullConversion { get; set; }
	}
}
