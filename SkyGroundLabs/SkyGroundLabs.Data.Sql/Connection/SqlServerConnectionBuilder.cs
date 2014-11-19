using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Connection
{
	public class SqlServerConnectionBuilder : IConnectionBuilder
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string DataSource { get; set; }
		public string InitialCatalog { get; set; }
		public bool MultipleActiveResultSets { get; set; }

		public string BuildConnectionString()
		{
			return string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets={4}",
				DataSource,
				InitialCatalog,
				Username,
				Password,
				MultipleActiveResultSets ? "true" : "false");
		}
	}
}
