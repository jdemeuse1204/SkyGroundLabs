using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Data.Tables;

namespace iONWeb.Business.Clocking
{
	public class UserClockingPair
	{
		#region Properties
		public Data.Tables.Clocking InPunch { get; set; }

		public Data.Tables.Clocking OutPunch { get; set; }

		public string Name { get; set; }

		public long UserID { get; set; }

		public long ManagerUserID { get; set; }

		public double Longitude { get; set; }

		public double Latitude { get; set; }
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
