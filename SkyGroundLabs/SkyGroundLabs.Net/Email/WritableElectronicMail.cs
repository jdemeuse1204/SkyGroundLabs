using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAGetMail;

namespace SkyGroundLabs.Net.Email
{
	public class WritableElectronicMail : ElectronicMail
	{
		public WritableElectronicMail() : base() { }

		public void Set(Mail mail)
		{
			HtmlBody = mail.HtmlBody;
			TextBody = mail.TextBody;
		}

		public string HtmlBody { get; set; }
		public string TextBody { get; set; }
	}
}
