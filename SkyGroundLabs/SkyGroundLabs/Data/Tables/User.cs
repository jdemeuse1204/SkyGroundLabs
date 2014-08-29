using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace SkyGroundLabs.Data.Tables
{
	[Table("Users")]
	public class User : DbTableEquatable<IDbTableEquatable<long>>
	{
		public User()
		{
			UserRoleTypeID = 1;
			ManagerUserID = 0;
			LastAuthenticationDate = Defaults.MinDateTime;
		}

		public long ID { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public int LoginAttempts { get; set; }

		public bool ForcePasswordChange { get; set; }

		public bool IsLocked { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string DisplayName { get; set; }

		public string Street { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Zip { get; set; }

		public string Phone1 { get; set; }

		public string Phone1Carrier { get; set; }

		public string Phone2 { get; set; }

		public string Phone2Carrier { get; set; }

		public string Email { get; set; }

		public string DatabasePassword { get; set; }

		public long UserRoleTypeID { get; set; }

		public string PIN { get; set; }

		public long ManagerUserID { get; set; }

		public string SecurityQuestion { get; set; }

		public string SecurityAnswer { get; set; }

		public DateTime LastAuthenticationDate { get; set; }
	}
}
