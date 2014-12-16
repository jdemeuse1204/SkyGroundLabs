using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Commands.Support
{
	public interface ISqlBuilder
	{
		void Table(string tableName);
		SqlCommand BuildCommand(SqlConnection connection);
	}
}
