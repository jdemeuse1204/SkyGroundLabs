using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ion.Data;

namespace OnlineIonTest
{
	public class ReusableContext : DbContext
	{
		public ReusableContext() : base("Data Source=lin.arvixe.com;Initial Catalog=iONWebDataStore_Live;User ID=jdemeuse1204;Password=aiwa1122;MultipleActiveResultSets=true") { }
	}
}
