using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iONWeb.Data;

namespace iONWeb_Test
{
	public class ReusableContext : DbContext
	{
		public ReusableContext() : base("Data Source=lin.arvixe.com;Initial Catalog=iONWebDataStore_Live;User ID=jdemeuse1204;Password=aiwa1122;MultipleActiveResultSets=true") { }
	}
}
