using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Connection
{
	public interface IConnectionBuilder
	{
		string Username { get; set; }
		string Password { get; set; }
	}
}
