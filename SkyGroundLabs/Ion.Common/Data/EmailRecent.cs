using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("EmailRecents")]
	public class EmailRecent 
	{
		public long ID { get; set; }

		public long UserID { get; set; }

		public string Email { get; set; }
	}
}
