using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Data;

namespace iONWeb.Business.Clocking
{
	public class iONClockingService
	{
		#region Properties
		private iONCredentials _credentials { get; set; }
		#endregion

		#region Constructor
		public iONClockingService(iONCredentials credentials)
		{
			_credentials = credentials;
		}
		#endregion

		#region Methods
		private Guid _getNewPairingID()
		{
			using (var context = new ClockingContext(_credentials))
			{
				var result = Guid.NewGuid();

				while (context.Clockings.Select(w => w.PairingID).Contains(result))
				{
					result = Guid.NewGuid();
				}

				return result;
			}
		}

		public void ClockIn()
		{
			using (var context = new ClockingContext(_credentials))
			{
				var inPunch = new Data.Tables.Clocking();
				inPunch.PunchTime = DateTime.Now;
				inPunch.PunchTimeRounded = DateTime.Now;
				inPunch.OriginalPunchTime = DateTime.Now;
				inPunch.PunchType = iONWeb.Business.Clocking.PunchType.In;
				inPunch.PairingID = _getNewPairingID();
				context.Clockings.Add(inPunch);

				var outPunch = new Data.Tables.Clocking();
				outPunch.PunchTime = DateTime.Now;
				outPunch.PunchTimeRounded = DateTime.Now;
				outPunch.OriginalPunchTime = DateTime.Now;
				outPunch.PunchType = iONWeb.Business.Clocking.PunchType.Out;
				outPunch.PairingID = inPunch.PairingID;
				context.Clockings.Add(outPunch);

				context.SaveChanges();
			}
		}
		#endregion

		// Connection to only be used with events
		private class ClockingContext : DbContext
		{
			public ClockingContext(iONCredentials credentials)
				: base(credentials.Server, credentials.Database, credentials.Username, credentials.Password)
			{

			}
		}
	}
}
