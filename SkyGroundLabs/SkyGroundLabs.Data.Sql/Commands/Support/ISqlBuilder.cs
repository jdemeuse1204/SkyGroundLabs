using System.Data.SqlClient;

namespace SkyGroundLabs.Data.Sql.Commands.Support
{
	public interface ISqlBuilder
	{
		void Table(string tableName);
		SqlCommand Build(SqlConnection connection);
	}
}
