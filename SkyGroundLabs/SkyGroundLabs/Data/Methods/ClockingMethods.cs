using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Business;
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

		#region Get Punches
		public UserClocking GetLastPunch(long userID)
		{
			return _context.UserClockings.Where(w => w.UserID == userID).OrderByDescending(w => w.ID).FirstOrDefault();
		}

		public UserClocking GetLastInPunch(long userID)
		{
			return _context.UserClockings.Where(w => w.UserID == userID && w.IsInPunch == true).OrderByDescending(w => w.PunchTime).FirstOrDefault();
		}

		public UserClocking GetLastOutPunch(long userID)
		{
			return _context.UserClockings.Where(w => w.UserID == userID && w.IsInPunch == false).OrderByDescending(w => w.PunchTime).FirstOrDefault();
		}

		public UserClocking GetLastOutPunch(long userID, Guid pairingID)
		{
			return _context.UserClockings.Where(w => w.UserID == userID && w.IsInPunch == false && w.PairingID == pairingID).OrderByDescending(w => w.PunchTime).FirstOrDefault();
		}

		public UserClocking GetPunch(Guid pairingID, bool isInPunch)
		{
			return _context.UserClockings.Where(w => w.PairingID == pairingID && w.IsInPunch == isInPunch).FirstOrDefault();
		}

		#endregion

		#region Punching
		public bool IsLastClockingInPunch(long userID)
		{
			UserClocking clocking = GetLastPunch(userID);

			if (clocking == null)
			{
				return false;
			}

			return clocking.IsInPunch;
		}

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

		public UserClocking PunchIn(long userID, DateTime dateTime, GeoLocation location, string specialCode = "", long appointmentID = 0)
		{
			UserClocking clocking = new UserClocking();
			clocking.AppointmentID = appointmentID;
			clocking.IsApproved = false;
			clocking.IsInPunch = true;
			clocking.PunchTime = dateTime;
			clocking.PunchTimeRounded = ClockingServices.Round(dateTime);
			clocking.SpecialCode = specialCode;
			clocking.UserID = userID;
			clocking.PairingID = _getNewGuid();
			clocking.OriginalPunchTime = dateTime;
			clocking.IsAdjusted = false;
			clocking.Longitude = location.Longitude;
			clocking.Latitude = location.Latitude;

			_context.SaveChanges(clocking);

			return clocking;
		}

		public UserClocking PunchOut(long userID, DateTime dateTime, GeoLocation location, string specialCode = "", long appointmentID = 0)
		{
			UserClocking clocking = new UserClocking();
			clocking.AppointmentID = appointmentID;
			clocking.IsApproved = false;
			clocking.IsInPunch = false;
			clocking.PunchTime = dateTime;
			clocking.PunchTimeRounded = ClockingServices.Round(dateTime);
			clocking.SpecialCode = specialCode;
			clocking.UserID = userID;
			clocking.PairingID = GetLastInPunch(userID).PairingID;
			clocking.OriginalPunchTime = dateTime;
			clocking.IsAdjusted = false;
			clocking.Longitude = location.Longitude;
			clocking.Latitude = location.Latitude;

			_context.SaveChanges(clocking);

			return clocking;
		}

		public bool HasOutPunch(Guid pairingID)
		{
			return _context.UserClockings.Where(w => w.PairingID == pairingID && w.IsInPunch == false).Count() > 0;
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

		#region Get Clockings
		public IEnumerable<UserClocking> GetClockings(DateTime start, DateTime end, long userID)
		{
			return _context.UserClockings.Where(w => w.UserID == userID &&
				w.PunchTime >= start && w.PunchTime <= end).OrderBy(w => w.PunchTime).ThenBy(w => w.PairingID).ThenByDescending(w => w.IsInPunch);
		}

		public IEnumerable<UserClocking> GetClockings(DateTime start, DateTime end)
		{
			return _context.UserClockings.Where(w => w.PunchTime >= start && w.PunchTime <= end).OrderBy(w => w.PunchTime).ThenBy(w => w.PairingID).ThenByDescending(w => w.IsInPunch);
		}

		public IEnumerable<UserClocking> GetAllClockings(long userID)
		{
			return _context.UserClockings.Where(w => w.UserID == userID).OrderBy(w => w.PunchTime).ThenBy(w => w.PairingID).ThenByDescending(w => w.IsInPunch);
		}

		private UserClocking _getClocking(dynamic d)
		{
			return new UserClocking()
			{
				IsInPunch = d.IsInPunch,
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
								 c.IsInPunch,
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

			foreach (var clocking in clockings.OrderBy(w => w.UserID).ThenBy(w => w.PairingID).ThenByDescending(w => w.IsInPunch).ThenBy(w => w.PunchTime))
			{
				// make sure we only create pairs with in and out punches
				if (clocking.IsInPunch && !HasOutPunch(clocking.PairingID))
				{
					continue;
				}

				if (clocking.IsInPunch)
				{
					pair = new UserClockingPair();
					pair.InPunch = _getClocking(clocking);
					pair.UserID = clocking.UserID;
					pair.ManagerUserID = clocking.ManagerUserID;
					pair.Name = clocking.Username;

					if (!clockings.Where(w => w.PairingID == clocking.PairingID).Select(w => w.IsInPunch).Contains(false))
					{
						// outpunch is outside of datetime parameters, need to add manually
						pair.OutPunch = _context.UserClockings.Where(w => w.PairingID == clocking.PairingID
							&& w.IsInPunch == false).FirstOrDefault();
						pair.UserID = clocking.UserID;
						pair.ManagerUserID = clocking.ManagerUserID;
						pair.Name = clocking.Username;
						results.Add(pair);
						pair = new UserClockingPair();
					}
				}
				else
				{
					// make sure the inpunch was in the list
					if (!clockings.Where(w => w.PairingID == clocking.PairingID).Select(w => w.IsInPunch).Contains(true))
					{
						// inpunch is outside of datetime parameters, need to add manually
						pair = new UserClockingPair();
						pair.InPunch = _context.UserClockings.Where(w => w.PairingID == clocking.PairingID
							&& w.IsInPunch == true).FirstOrDefault();
						pair.UserID = clocking.UserID;
						pair.ManagerUserID = clocking.ManagerUserID;
						pair.Name = clocking.Username;
					}

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
