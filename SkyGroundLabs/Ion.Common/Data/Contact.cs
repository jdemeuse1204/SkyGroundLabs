using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("Contacts")]
	public class Contact
	{
		public Contact()
		{
			Zip = "";
			DateEdited = DateTime.Now;
			DateEntered = DateTime.Now;
			AppointmentCount = 0;
			IsLocked = false;
			LockedByUserID = 0;
		}

		public long ID { get; set; }

		public string FirstName1 { get; set; }

		public string FirstName2 { get; set; }

		public string FirstName3 { get; set; }

		public string LastName1 { get; set; }

		public string LastName2 { get; set; }

		public string LastName3 { get; set; }

		public string Email1 { get; set; }

		public string Email2 { get; set; }

		public string Street { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Zip { get; set; }

		public string Phone1 { get; set; }

		public string Phone2 { get; set; }

		public string Phone3 { get; set; }

		public string Phone1Type { get; set; }

		public string Phone2Type { get; set; }

		public string Phone3Type { get; set; }

		public DateTime DateEdited { get; set; }

		public string Comments { get; set; }

		public string DisplayName1 { get; set; }

		public string DisplayName2 { get; set; }

		public string DisplayName3 { get; set; }

		public string DisplayAddress { get; set; }

		public DateTime DateEntered { get; set; }

		public long AppointmentCount { get; set; }

		public bool IsLocked { get; set; }

		public long LockedByUserID { get; set; }

		public long CreatedByID { get; set; }

		public long EditedByID { get; set; }
	}
}
