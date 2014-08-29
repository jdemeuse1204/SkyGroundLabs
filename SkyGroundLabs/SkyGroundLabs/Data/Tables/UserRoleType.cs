using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace SkyGroundLabs.Data.Tables
{
	[Table("UserRoleTypes")]
	public class UserRoleType : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public string Role { get; set; }

		public string Description { get; set; }
	}
}
