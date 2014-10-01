using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("EmailRecents")]
	public class EmailRecent : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public long UserID { get; set; }

		public string Email { get; set; }
	}
}
