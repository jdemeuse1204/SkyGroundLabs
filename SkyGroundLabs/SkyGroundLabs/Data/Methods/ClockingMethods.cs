using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Business;
using SkyGroundLabs.Business.Enumeration;
using SkyGroundLabs.Business.Services;
using SkyGroundLabs.Data.Tables;

namespace SkyGroundLabs.Data.Methods
{
	public class ClockingMethods
	{
		#region Properties
		private DbContext _context { get; set; }
		#endregion

		#region Constructor
		public ClockingMethods(DbContext context)
		{
			_context = context;
		}
		#endregion

		#region Methods

		#region Punching
		public void AdjustClocking(long clockingID, long userID, DateTime newTime)
		{
			var clocking = _context.UserClockings.Find(clockingID);

			if (clocking != null)
			{
				clocking.PunchTime = newTime;
				clocking.PunchTimeRounded = ClockingServices.Round(newTime);
				clocking.IsAdjusted = true;
				clocking.AdjustedUserID = userID;
			}

			_context.SaveChanges(clocking);
		}

		public UserClocking PunchIn(long userID, DateTime dateTime, GeoLocation location, string GMTOffset, string timeZoneName, string specialCode = "", long appointmentID = 0)
		{
			UserClocking inPunch = new UserClocking();
			inPunch.AppointmentID = appointmentID;
			inPunch.IsApproved = false;
			inPunch.PunchType = PunchType.In;
			inPunch.PunchTime = dateTime;
			inPunch.PunchTimeRounded = ClockingServices.Round(dateTime);
			inPunch.SpecialCode = specialCode;
			inPunch.UserID = userID;
			inPunch.PairingID = _getNewGuid();
			inPunch.OriginalPunchTime = dateTime;
			inPunch.IsAdjusted = false;
			inPunch.Longitude = location.Longitude;
			inPunch.Latitude = location.Latitude;
			inPunch.PunchTimeZoneName = timeZoneName;
			inPunch.PunchTimeZoneOffset = GMTOffset;

			_context.SaveChanges(inPunch);

			// Save the out punch with no time in it
			UserClocking outPunch = new UserClocking();
			outPunch.AppointmentID = appointmentID;
			outPunch.IsApproved = false;
			outPunch.PunchType = PunchType.Out;
			outPunch.PunchTime = Defaults.MinDateTime;
			outPunch.PunchTimeRounded = Defaults.MinDateTime;
			outPunch.SpecialCode = specialCode;
			outPunch.UserID = userID;
			outPunch.PairingID = inPunch.PairingID;
			outPunch.OriginalPunchTime = Defaults.MinDateTime;
			outPunch.IsAdjusted = false;
			outPunch.Longitude = 0;
			outPunch.Latitude = 0;
			outPunch.PunchTimeZoneName = "";
			outPunch.PunchTimeZoneOffset = "";

			_context.SaveChanges(outPunch);

			return inPunch;
		}

		public IEnumerable<UserClocking> PunchBoth(long userID, DateTime indateTime, DateTime outdateTime, GeoLocation location, string GMTOffset, string timeZoneName, string specialCode = "", long appointmentID = 0)
		{
			var punchSet = new List<UserClocking>();

			UserClocking inPunch = new UserClocking();
			inPunch.AppointmentID = appointmentID;
			inPunch.IsApproved = false;
			inPunch.PunchType = PunchType.In;
			inPunch.PunchTime = indateTime;
			inPunch.PunchTimeRounded = ClockingServices.Round(indateTime);
			inPunch.SpecialCode = specialCode;
			inPunch.UserID = userID;
			inPunch.PairingID = _getNewGuid();
			inPunch.OriginalPunchTime = indateTime;
			inPunch.IsAdjusted = false;
			inPunch.Longitude = location.Longitude;
			inPunch.Latitude = location.Latitude;
			inPunch.PunchTimeZoneName = timeZoneName;
			inPunch.PunchTimeZoneOffset = GMTOffset;

			_context.SaveChanges(inPunch);

			// Save the out punch with no time in it
			UserClocking outPunch = new UserClocking();
			outPunch.AppointmentID = appointmentID;
			outPunch.IsApproved = false;
			outPunch.PunchType = PunchType.Out;
			outPunch.PunchTime = outdateTime;
			outPunch.PunchTimeRounded = ClockingServices.Round(outdateTime);
			outPunch.SpecialCode = specialCode;
			outPunch.UserID = userID;
			outPunch.PairingID = inPunch.PairingID;
			outPunch.OriginalPunchTime = outdateTime;
			outPunch.IsAdjusted = false;
			outPunch.Longitude = location.Longitude;
			outPunch.Latitude = location.Latitude;
			outPunch.PunchTimeZoneName = timeZoneName;
			outPunch.PunchTimeZoneOffset = GMTOffset;

			_context.SaveChanges(outPunch);

			punchSet.Add(inPunch);
			punchSet.Add(outPunch);

			return punchSet;
		}

		public UserClocking PunchOut(long userID, DateTime dateTime, GeoLocation location, string GMTOffset, string timeZoneName, string specialCode = "", long appointmentID = 0)
		{
			var outPunch = _context.UserClockings.Where(w => w.UserID == userID
				&& w.PunchType == PunchType.Out
				&& w.PunchTime == Defaults.MinDateTime).FirstOrDefault();

			var inPunch = _context.UserClockings.Where(w => w.PairingID == outPunch.PairingID 
				&& w.PunchType == PunchType.In).FirstOrDefault();

			if (outPunch == null)
			{
				throw new Exception(string.Format("Out punch not found for user {0}", userID));
			}

			// Adjust datetime if the in punch and out punch are in different time zones
			// adjust the out punch to the inpuch time zone
			if (GMTOffset != inPunch.PunchTimeZoneOffset)
			{
				// get time difference
				var inPunchOffset = Convert.ToDouble(inPunch.PunchTimeZoneOffset.Replace("GMT", ""));
				var outPunchOffset = Convert.ToDouble(GMTOffset.Replace("GMT", ""));
				var delta = (inPunchOffset - outPunchOffset) / 100;
				dateTime = dateTime.AddHours(delta);
			}

			outPunch.AppointmentID = appointmentID;
			outPunch.IsApproved = false;
			outPunch.PunchType = PunchType.Out;
			outPunch.PunchTime = dateTime;
			outPunch.PunchTimeRounded = ClockingServices.Round(dateTime);
			outPunch.SpecialCode = specialCode;
			outPunch.UserID = userID;
			outPunch.PairingID = GetLastInPunch(userID).PairingID;
			outPunch.OriginalPunchTime = dateTime;
			outPunch.IsAdjusted = false;
			outPunch.Longitude = location.Longitude;
			outPunch.Latitude = location.Latitude;
			outPunch.PunchTimeZoneName = timeZoneName;
			outPunch.PunchTimeZoneOffset = GMTOffset;

			_context.SaveChanges(outPunch);

			return outPunch;
		}

		public bool HasOutPunch(Guid pairingID)
		{
			return _context.UserClockings.Where(w => w.PairingID == pairingID && w.PunchType == PunchType.Out && w.PunchTime != Defaults.MinDateTime).Count() > 0;
		}

		private Guid _getNewGuid()
		{
			Guid guid = Guid.NewGuid();

			while (_context.UserClockings.Select(w => w.PairingID).Contains(guid))
			{
				guid = Guid.NewGuid();
			}

			return guid;
		}
		#endregion

		#region Get Punches
		public UserClocking GetLastPunch(long userID)
		{
			return _context.UserClockings.Where(w => w.UserID == userID && w.PunchTime != Defaults.MinDateTime).OrderByDescending(w => w.ID).FirstOrDefault();
		}

		public PunchType GetLastPunchType(long userID)
		{
			return GetLastPunch(userID).PunchType;
		}

		public UserClocking GetLastInPunch(long userID)
		{
			return _context.UserClockings.Where(w => w.UserID == userID && w.PunchType == PunchType.In).OrderByDescending(w => w.PunchTime).FirstOrDefault();
		}

		public UserClocking GetLastOutPunch(long userID)
		{
			return _context.UserClockings.Where(w => w.UserID == userID && w.PunchType == PunchType.Out && w.PunchTime != Defaults.MinDateTime).OrderByDescending(w => w.PunchTime).FirstOrDefault();
		}

		public UserClocking GetLastOutPunch(long userID, Guid pairingID)
		{
			return _context.UserClockings.Where(w => w.UserID == userID && w.PunchType == PunchType.Out && w.PairingID == pairingID && w.PunchTime != Defaults.MinDateTime).OrderByDescending(w => w.PunchTime).FirstOrDefault();
		}

		public UserClocking GetPunch(Guid pairingID, PunchType punchType)
		{
			return _context.UserClockings.Where(w => w.PairingID == pairingID && w.PunchType == punchType).FirstOrDefault();
		}
		#endregion

		#region Get Clockings
		/// <summary>
		/// Makes sure the clockings have an in and out punch in case 
		/// one was missed because of the date range
		/// </summary>
		/// <param name="clockings"></param>
		private void _aduitClockingPairs(List<UserClocking> clockings)
		{
			var additions = new List<UserClocking>();

			foreach (var item in clockings.Select(w => w.PairingID).Distinct())
			{
				if (clockings.Where(w => w.PairingID == item).Count() == 1)
				{
					var single = clockings.Where(w => w.PairingID == item).First();
					var punchType = PunchType.Out;

					if (single.PunchType == PunchType.Out)
					{
						// Missing the in punch
						punchType = PunchType.In;
					}

					var punch = _context.UserClockings.Where(w => w.PairingID == item && w.PunchType == punchType).First();
					additions.Add(punch);
				}
			}

			clockings.AddRange(additions);
		}

		public IEnumerable<UserClocking> GetClockings(DateTime start, DateTime end, long userID)
		{
			// make sure to get full pairs
			var clockings = _context.UserClockings.Where(w => w.UserID == userID
				&& DbFunctions.TruncateTime(w.PunchTime) >= DbFunctions.TruncateTime(start)
				&& DbFunctions.TruncateTime(w.PunchTime) <= DbFunctions.TruncateTime(end)).ToList();

			// make sure in punches and out punches have correct pairings
			_aduitClockingPairs(clockings);

			return clockings.OrderBy(w => w.PunchTime).ThenBy(w => w.PairingID).ThenByDescending(w => w.PunchType);
		}

		public IEnumerable<UserClocking> GetClockings(DateTime start, DateTime end)
		{
			// make sure to get full pairs
			var clockings = _context.UserClockings.Where(w => DbFunctions.TruncateTime(w.PunchTime) >= DbFunctions.TruncateTime(start)
				&& DbFunctions.TruncateTime(w.PunchTime) <= DbFunctions.TruncateTime(end)).ToList();

			// make sure in punches and out punches have correct pairings
			_aduitClockingPairs(clockings);

			return clockings.OrderBy(w => w.PunchTime).ThenBy(w => w.PairingID).ThenByDescending(w => w.PunchType);
		}

		public IEnumerable<UserClocking> GetAllClockings(long userID)
		{
			// make sure to get full pairs
			var clockings = _context.UserClockings.Where(w => w.UserID == userID).ToList();

			// make sure in punches and out punches have correct pairings
			_aduitClockingPairs(clockings);

			return clockings.OrderBy(w => w.PunchTime).ThenBy(w => w.PairingID).ThenByDescending(w => w.PunchType);
		}

		private UserClocking _getClocking(dynamic d)
		{
			return new UserClocking()
			{
				PunchType = PunchType.In,
				ID = d.ID,
				UserID = d.UserID,
				PunchTime = d.PunchTime,
				PunchTimeRounded = d.PunchTimeRounded,
				SpecialCode = d.SpecialCode,
				AppointmentID = d.AppointmentID,
				IsApproved = d.IsApproved,
				PairingID = d.PairingID,
				OriginalPunchTime = d.OriginalPunchTime,
				IsAdjusted = d.IsAdjusted,
				AdjustedUserID = d.AdjustedUserID
			};
		}
		#endregion

		#region Get Pairs
		public IEnumerable<UserClockingPair> GetPairs(long userID)
		{
			var results = _allPairs(Defaults.MinDateTime, DateTime.MaxValue);

			if (results.Count() != 0)
			{
				return results.Where(w => w.UserID == userID);
			}

			return results;
		}

		public IEnumerable<UserClockingPair> GetPairs(DateTime start, DateTime end)
		{
			return _allPairs(start, end);
		}

		public IEnumerable<UserClockingPair> GetPairs(DateTime start, DateTime end, long userID)
		{
			var results = _allPairs(start, end);

			if (results.Count() != 0)
			{
				return results.Where(w => w.UserID == userID);
			}

			return results;
		}

		private IEnumerable<UserClockingPair> _allPairs(DateTime start, DateTime end)
		{
			var results = new List<UserClockingPair>();
			var clockings = (from c in GetClockings(start, end)
							 join u in _context.Users on c.UserID equals u.ID
							 select new
							 {
								 c.PunchType,
								 c.ID,
								 c.UserID,
								 c.PunchTime,
								 c.PunchTimeRounded,
								 c.SpecialCode,
								 c.AppointmentID,
								 c.IsApproved,
								 c.PairingID,
								 c.OriginalPunchTime,
								 c.IsAdjusted,
								 c.AdjustedUserID,
								 u.ManagerUserID,
								 Username = u.FirstName + (string.IsNullOrWhiteSpace(u.LastName) ? "" : " " + u.LastName)
							 });

			var pair = new UserClockingPair();

			foreach (var clocking in clockings.OrderBy(w => w.PairingID).ThenBy(w => w.PunchType))
			{
				if (clocking.PunchType == PunchType.In)
				{
					pair = new UserClockingPair();
					pair.InPunch = _getClocking(clocking);
					pair.UserID = clocking.UserID;
					pair.ManagerUserID = clocking.ManagerUserID;
					pair.Name = clocking.Username;
				}
				else
				{
					pair.OutPunch = _getClocking(clocking);
					pair.UserID = clocking.UserID;
					pair.ManagerUserID = clocking.ManagerUserID;
					pair.Name = clocking.Username;
					results.Add(pair);
					pair = new UserClockingPair();
				}
			}

			return results;
		}
		#endregion

		#endregion
	}
}
