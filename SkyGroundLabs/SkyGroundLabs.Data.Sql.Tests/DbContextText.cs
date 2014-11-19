using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ion.Common.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkyGroundLabs.Data.Sql.Connection;

namespace SkyGroundLabs.Data.Sql.Tests
{
	[TestClass]
	public class DbContextText
	{
		private SqlServerConnectionBuilder _getConnectionBuilder()
		{
			return new SqlServerConnectionBuilder()
			{
				DataSource = "",
				InitialCatalog = "",
				MultipleActiveResultSets = true,
				Password = "",
				Username = ""
			};
		}

		[TestMethod]
		public void TestFindNull()
		{
			var context = new DbContext(_getConnectionBuilder());

			var contact = context.Find<Contact>(0);

			Assert.AreEqual(null, contact);
		}

		[TestMethod]
		public void TestInsert()
		{
			var context = new DbContext(_getConnectionBuilder());

			var contact = new Contact();
			contact.FirstName1 = "James";
			contact.LastName1 = "Demeuse";
			contact.Phone1 = "(414) 530-3086";
			contact.State = "WI";
			contact.Street = "2424 Springdale Rd Apt 203";
			contact.City = "Waukesha";
			contact.Zip = "53186";
			contact.DateEntered = DateTime.Now;
			contact.DateEdited = DateTime.Now;

			context.SaveChanges<Contact>(contact);

			Assert.AreNotEqual(null, contact);
			Assert.AreNotEqual(contact.ID, 0);
		}

		[TestMethod]
		public void TestUpdate()
		{
			var context = new DbContext(_getConnectionBuilder());
			var ID = 28;
			var contact = new Contact();
			contact.ID = ID;
			contact.FirstName1 = "James";
			contact.LastName1 = "Demeuse";
			contact.Phone1 = "(414) 530-3086";
			contact.State = "WI";
			contact.Street = "2424 Springdale Rd Apt 203";
			contact.City = "Waukesha";
			contact.Zip = "53186";
			contact.DateEntered = DateTime.Now;
			contact.DateEdited = DateTime.Now;

			context.SaveChanges<Contact>(contact);

			Assert.AreNotEqual(contact.ID, 0);

			contact = context.Find<Contact>(ID);

			Assert.AreEqual("James", contact.FirstName1);
		}

		[TestMethod]
		public void TestUConnectionBuilder()
		{
			var builder = new SqlServerConnectionBuilder();
			builder.DataSource = "Server";
			builder.InitialCatalog = "Database";
			builder.MultipleActiveResultSets = true;
			builder.Password = "PASSWORD";
			builder.Username = "USERNAME";

			var builderResult = builder.BuildConnectionString();
			Assert.AreEqual(builderResult,
				"Data Source=Server;Initial Catalog=Database;User ID=USERNAME;Password=PASSWORD;MultipleActiveResultSets=true");
		}
	}
}
