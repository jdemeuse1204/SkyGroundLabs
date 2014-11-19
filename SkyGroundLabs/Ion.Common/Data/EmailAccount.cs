using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Mapping;

namespace Ion.Common.Data
{
	[Table("EmailAccounts")]
	public class EmailAccount 
	{
		public long ID { get; set; }

		public string Account { get; set; }

		public string SmtpServerAddress { get; set; }

		public string Password { get; set; }

		public int SentPort { get; set; }

		public int RetreivePort { get; set; }

		public bool SendUseSSL { get; set; }

		public bool RetreiveUseSSL { get; set; }

		public string Pop3ServerAddress { get; set; }

		public string AuthenticationKey { get; set; }

		public long UserID { get; set; }

		public string Signature { get; set; }
	}
}
