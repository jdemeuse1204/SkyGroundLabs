using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("UserRoleAccessPages")]
	public class UserRoleAccessPages
	{
		public long ID { get; set; }

		public string PageName { get; set; }
	}
}
