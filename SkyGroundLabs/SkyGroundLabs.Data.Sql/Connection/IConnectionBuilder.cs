namespace SkyGroundLabs.Data.Sql.Connection
{
	public interface IConnectionBuilder
	{
		string Username { get; set; }
		string Password { get; set; }

		string BuildConnectionString();
	}
}
