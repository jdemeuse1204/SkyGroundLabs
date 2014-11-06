using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Functions
{
	public class DbFunctions
	{
		public static string Replace(string valueToReplace, string newValue)
		{
			var item = new FunctionReplace(valueToReplace, newValue);

			return item.Get();
		}

		private static string Upper()
		{
			var item = new FunctionUpper();
			return item.Get();
		}

		class FunctionReplace : IDbFunction
		{
			private string _valueToReplace { get; set; }
			private string _newValue { get; set; }

			public FunctionReplace(string valueToReplace, string newValue)
			{
				_valueToReplace = valueToReplace;
				_newValue = newValue;
			}

			public string Get()
			{
				return string.Format("REPLACE({0},'{1}','{2}') as {0}", "{0}", _valueToReplace, _newValue);
			}
		}

		class FunctionUpper : IDbFunction
		{
			public string Get()
			{
				return "UPPER({0}) as {0}";
			}
		}
	}
}
