using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Functions
{
	public class DbFunctions
	{
		public static string Replace(string value, string valueToReplace, string newValue)
		{
			return string.Format("REPLACE({0},'{1}','{2}') as {0}", "{0}", value, valueToReplace, newValue);
		}

		private static string Upper(string value)
		{
			return string.Format("UPPER({0}) as {0}", value);
		}
	}
}
