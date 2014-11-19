using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("UserRoleAccess")]
	public class UserRoleAccess
	{
		public long ID { get; set; }

		public long UserRoleAccessPageID { get; set; }

		public long UserRoleID { get; set; }
	}
}
