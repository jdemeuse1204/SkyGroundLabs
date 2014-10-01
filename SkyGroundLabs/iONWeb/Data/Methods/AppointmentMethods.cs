using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Data.Tables;
using iONWeb.Business.Extension;
using iONWeb.Business.Services;

namespace iONWeb.Data.Methods
{
	public class AppointmentMethods
	{
		private DbContext _context { get; set; }

		public AppointmentMethods(DbContext context)
		{
			_context = context;
		}

		public bool AreDatesValidForScheduling(Appointment appointment)
		{
			if (appointment.StartTime != "12:00 AM" &&
				appointment.EndTime != "12:00 AM" &&
				appointment.ScheduledDate.Date != Defaults.MinDateTime.Date)
			{
				return true;
			}

			return false;
		}

		public dynamic AreRequrementsMet(Appointment appointment)
		{
			dynamic result = new ExpandoObject();
			result.Contents = string.Empty;
			result.CanSchedule = false;

			foreach (Indicator indicator in _context.Indicators)
			{
				if (!indicator.Value)
					continue;

				switch (indicator.ID)
				{
					case 1:
						if (appointment.MoveTypeID.IsCurrentValueNullOrEmptyOrZero())
						{
							result.Contents = "Move Type Required!";
						}
						break;
					case 2:
						if (appointment.StoreID.IsCurrentValueNullOrEmptyOrZero())
						{
							result.Contents = "Store Required!";
						}
						break;
					case 3:
						if (appointment.SafeID.IsCurrentValueNullOrEmptyOrZero())
						{
							result.Contents = "Safe Required!";
						}
						break;
					case 4:
						if (appointment.FloorID.IsCurrentValueNullOrEmptyOrZero())
						{
							result.Contents = "Floor Required!";
						}
						break;
					case 5:
						break;
					case 6:
						string doStreet = appointment.DropoffStreet.ToStringOvr();
						string doCity = appointment.DropoffCity.ToStringOvr();
						string doState = appointment.DropoffState.ToStringOvr();
						string doZip = appointment.DropoffZip.ToStringOvr();

						if (String.IsNullOrWhiteSpace(ContactServices.BuildAddress(doStreet, doCity, doState, doZip)))
						{
							result.Contents = "Dropoff Address Required!";
						}
						break;
					case 7:
						if (appointment.DropoffZip.IsCurrentValueNullOrEmptyOrZero())
						{
							result.Contents = "Dropoff Zip Code Required!";
						}
						break;
					case 8:
						if (appointment.PickupZip.IsCurrentValueNullOrEmptyOrZero())
						{
							result.Contents = "Pickup Zip Code Required!";
						}
						break;
					case 9:
						string puStreet = appointment.PickupStreet.ToStringOvr();
						string puCity = appointment.PickupCity.ToStringOvr();
						string puState = appointment.PickupState.ToStringOvr();
						string puZip = appointment.PickupZip.ToStringOvr();

						if (String.IsNullOrWhiteSpace(ContactServices.BuildAddress(puStreet, puCity, puState, puZip)))
						{
							result.Contents = "Pickup Address Required!";
						}
						break;
					default:
						break;
				}
			}

			if (String.IsNullOrWhiteSpace(result.Contents))
			{
				result.CanSchedule = true;
			}

			return result;
		}
	}
}
