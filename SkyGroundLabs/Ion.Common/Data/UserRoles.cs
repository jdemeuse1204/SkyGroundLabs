using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("UserRoleTypes")]
	public class UserRoles 
	{
		public long ID { get; set; }

		public string Role { get; set; }

		public string Description { get; set; }
	}
}
