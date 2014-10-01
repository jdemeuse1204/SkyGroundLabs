using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("UserRoleAccessPages")]
	public class UserRoleAccessPages : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }

		public string PageName { get; set; }
	}
}
