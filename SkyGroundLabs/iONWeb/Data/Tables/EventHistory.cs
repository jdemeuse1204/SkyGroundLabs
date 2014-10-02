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
	[Table("EventHistory")]
	public class EventHistory : DbTableEquatable<IDbTableEquatable<Guid>>
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Key]
		[Column(Order = 0)]
		public Guid EventID { get; set; }

		public DateTime TransactionDate { get; set; }

		public EventHistoryType TransactionType { get; set; }

		public int UserID { get; set; }
	}
}
