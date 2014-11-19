using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("AppointmentContentsView")]
	public class AppointmentContentsView
	{
		public long ID { get; set; }

		public string Description { get; set; }

		public bool IsSelected { get; set; }

		public long Tag { get; set; }

		public string PrependTag { get; set; }
	}
}
