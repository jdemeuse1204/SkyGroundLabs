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
	[Table("CalendarHistory")]
	public class CalendarHistory : DbTableEquatable<IDbTableEquatable<Guid>>
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Key]
		[Column(Order = 0)]
		public Guid CalendarID { get; set; }

		public DateTime TransactionDate { get; set; }

		public CalendarHistoryType TransactionType { get; set; }

		public int UserID { get; set; }
	}
}
