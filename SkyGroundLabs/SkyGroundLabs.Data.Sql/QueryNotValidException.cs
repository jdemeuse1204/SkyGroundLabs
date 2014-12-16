using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql
{
	public class QueryNotValidException : Exception
	{
		public QueryNotValidException(string message)
			: base(message)
		{

		}
	}
}
