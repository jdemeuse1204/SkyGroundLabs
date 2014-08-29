using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Tasks
{
	public class TaskEventArgs : EventArgs
	{
		public string Action { get; set; }
		public string TaskName { get; set; }
		public object Parameters { get; set; }
	}
}
