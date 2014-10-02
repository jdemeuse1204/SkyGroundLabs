using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace iONWeb.Data.Tables
{
	[Table("Users")]
	public class User : DbTableEquatable<IDbTableEquatable<int>>
	{
		public int ID { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public int LoginAttempts { get; set; }

		public bool ForcePasswordChange { get; set; }

		public bool IsLocked { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Address1 { get; set; }

		public string Address2 { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Zip { get; set; }

		public string Phone1 { get; set; }

		public string Phone2 { get; set; }

		public string Email { get; set; }

		public int UserRoleID { get; set; }

		public int ManagerUserID { get; set; }

		public int CompanyID { get; set; }

		public string SecurityQuestion { get; set; }

		public string SecurityAnswer { get; set; }

		public DateTime LastAuthenticationDate { get; set; }
	}
}
