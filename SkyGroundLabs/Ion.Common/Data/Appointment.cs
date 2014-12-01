using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("Appointments")]
	public class Appointment 
	{
		public Appointment()
		{
			StartDate = Convert.ToDateTime("1/1/1900 12:00 AM");
			EndDate = Convert.ToDateTime("1/1/1900 12:00 AM");
			Background = "Black";
			DropoffZip = "";
			PickupZip = "";
			DateEntered = DateTime.Now;
			DateEdited = DateTime.Now;
		}

		public long ID { get; set; }

		public bool IsScheduled { get; set; }

		public bool IsAllDay { get; set; }

		public long FloorID { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public long CrewID { get; set; }

		public string Background { get; set; }

		public string DropoffStreet { get; set; }

		public string DropoffCity { get; set; }

		public string DropoffState { get; set; }

		public string DropoffZip { get; set; }

		public string PickupStreet { get; set; }

		public string PickupCity { get; set; }

		public string PickupState { get; set; }

		public string PickupZip { get; set; }

		public bool IsEmailed { get; set; }

		public DateTime DateEntered { get; set; }

		public DateTime DateEdited { get; set; }

		public long ContactID { get; set; }

		public string Notes { get; set; }

		public long SafeID { get; set; }

		public long StoreID { get; set; }

		public double Weight { get; set; }

		public long MoveTypeID { get; set; }

		public double Mileage { get; set; }

		public double Quote { get; set; }

		public string Title { get; set; }

		public long CreatedByID { get; set; }

		public long EditedByID { get; set; }
	}
}
