using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineiONWeb.Business
{
	public class UserAccess
	{
		public long ID { get; set; }
		public string PageName { get; set; }
		public bool HasAccess { get; set; }
	}
}