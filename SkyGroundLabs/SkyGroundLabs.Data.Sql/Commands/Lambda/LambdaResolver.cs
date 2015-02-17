using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using LambdaSqlBuilder;
using SkyGroundLabs.Data.Sql.Data;

namespace SkyGroundLabs.Data.Sql.Commands.Lambda
{
	public class LambdaResolver : ILambdaResolver
    {
		public LambdaResolutionResult Resolve<T>(Expression<Func<T, bool>> expression)
        {
			var resolution = new SqlLam<T>(expression);
			return new LambdaResolutionResult(resolution.QueryString, resolution.QueryParameters);
        }
    }
}
