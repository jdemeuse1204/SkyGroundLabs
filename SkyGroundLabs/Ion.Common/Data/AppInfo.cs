using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("AppInfo")]
	public class AppInfo
	{
		public long ID { get; set; }

		public int MajorVersion { get; set; }

		public int MinorVersion { get; set; }

		public int BuildVersion { get; set; }

		public int RevisionVersion { get; set; }

		public bool OverrideBuildVersionInfo { get; set; }

		public long OwnerID { get; set; }
	}
}
