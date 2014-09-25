using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace SkyGroundLabs.Data.Tables
{
	[Table("Appointments")]
	public class Appointment : DbTableEquatable<IDbTableEquatable<long>>
	{
		public Appointment()
		{
			IsScheduled = false;
			FloorID = 0;
			StartTime = "12:00 AM";
			EndTime = "12:00 AM";
			CrewID = 0;
			Background = "Black";
			DropoffZip = "";
			PickupZip = "";
			IsEmailed = false;
			DateEntered = DateTime.Now;
			DateEdited = DateTime.Now;
			ContactID = 0;
			SafeID = 0;
			StoreID = 0;
			Weight = 0d;
			Quote = 0d;
			ScheduledDate = Convert.ToDateTime("1/1/1900 12:00 AM");
		}

		public long ID { get; set; }

		public bool IsScheduled { get; set; }

		[Column("FloorLocationID")]
		public long FloorID { get; set; }

		public string StartTime { get; set; }

		public string EndTime { get; set; }

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

		public string GoogleCalendarUID { get; set; }

		public DateTime DateEntered { get; set; }

		public DateTime DateEdited { get; set; }

		public long ContactID { get; set; }

		public string Notes { get; set; }

		public long SafeID { get; set; }

		public long StoreID { get; set; }

		public double Weight { get; set; }

		public long MoveTypeID { get; set; }

		public DateTime ScheduledDate { get; set; }

		public string DisplayContents { get; set; }

		public double Mileage { get; set; }

		public double Quote { get; set; }

		public string DisplayTitle { get; set; }

		public long CreatedByID { get; set; }

		public long EditedByID { get; set; }
	}
}
