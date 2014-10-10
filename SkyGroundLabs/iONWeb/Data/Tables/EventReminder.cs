using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Data.Enumeration;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("EventReminders")]
	public class EventReminder : DbTableEquatable<IDbTableEquatable<Guid>>
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid ID { get; set; }

		public Guid EventID { get; set; }

		public int Hours { get; set; }

		public int Minutes { get; set; }

		public int Days { get; set; }

		/// <summary>
		/// Who the reminder is for.  There can be more than one person
		/// on the event, so we need to say who the reminder is for,
		/// a 0 indicates the reminder is for everyone
		/// </summary>
		public int UserID { get; set; }

		[Column("ReminderTypeID")]
		public NotificationType Type { get; set; }

		[NotMapped]
		public bool MarkedForRemoval { get; set; }  // if this is set then we need to remove the entry from the database
	}
}
