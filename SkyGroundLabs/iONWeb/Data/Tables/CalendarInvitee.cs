using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Data.Enumeration;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("CalendarInvitees")]
	public class CalendarInvitee : DbTableEquatable<IDbTableEquatable<Guid>>
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid ID { get; set; }

		public Guid CalendarID { get; set; }

		[Column("InviteeTypeID")]
		public InviteeType Type { get; set; }

		public int InviteeID { get; set; }

		public bool NotificationSent { get; set; }

		[NotMapped]
		public bool MarkedForRemoval { get; set; }  // if this is set then we need to remove the entry from the database
	}
}
