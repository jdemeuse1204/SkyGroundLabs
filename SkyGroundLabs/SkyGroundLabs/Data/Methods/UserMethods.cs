using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Tables;

namespace SkyGroundLabs.Data.Methods
{
	public class UserMethods
	{
		private DbContext _context { get; set; }

		public UserMethods(DbContext context)
		{
			_context = context;
		}

		public User GetUser(string username)
		{
			return _context.Users.Where(w => w.Username.ToUpper() == username.ToUpper()).FirstOrDefault();
		}

		public string GetNewPIN()
		{
			Random random = new Random();

			int rnd = random.Next(1000, 9999);

			// do not want duplicate pin numbers
			while (_context.Users.Select(w => w.PIN).Contains(rnd.ToString()))
			{
				rnd = random.Next(1000, 9999);
			}

			return rnd.ToString();
		}

		public User CreateUser()
		{
			User user = new User();

			user.PIN = GetNewPIN();

			return user;
		}
	}
}
