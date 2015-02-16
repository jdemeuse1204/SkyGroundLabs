using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Commands.Lambda
{
	public class LambdaResolutionResult
	{
		public LambdaResolutionResult(string queryString, IDictionary<string, object> queryParameters)
		{
			QueryString = queryString;
			QueryParameters = queryParameters;
		}

		public string QueryString { get; private set; }

		public IDictionary<string, object> QueryParameters { get; private set; }
	}
}
