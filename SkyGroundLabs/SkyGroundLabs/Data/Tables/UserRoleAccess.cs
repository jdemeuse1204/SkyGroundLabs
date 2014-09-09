using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace SkyGroundLabs.Data.Tables
{
	[Table("UserRoleAccess")]
	public class UserRoleAccess : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public long UserRoleAccessPageID { get; set; }

		public long UserRoleID { get; set; }
	}
}
