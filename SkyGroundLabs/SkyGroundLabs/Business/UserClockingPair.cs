using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Tables;

namespace SkyGroundLabs.Business
{
	public class UserClockingPair
	{
		#region Properties
		public UserClocking InPunch { get; set; }

		public UserClocking OutPunch { get; set; }

		public string Name { get; set; }

		public long UserID { get; set; }

		public long ManagerUserID { get; set; }
		#endregion

		#region Methods
		public double GetHours()
		{
			TimeSpan span = OutPunch.PunchTimeRounded - InPunch.PunchTimeRounded;
			return span.TotalHours;
		}
		#endregion
	}
}
