using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Data;

namespace SkyGroundLabs.Data.Sql.Commands.Lambda
{
	public interface ILambdaResolver
	{
		LambdaResolutionResult Resolve<T>(Expression<Func<T, bool>> expression);
	}
}
