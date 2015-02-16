using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using LambdaSqlBuilder;
using SkyGroundLabs.Data.Sql.Data;

namespace SkyGroundLabs.Data.Sql.Commands.Lambda
{
	public class LambdaResolver : ILambdaResolver
    {
		public LambdaResolutionResult Resolve<T>(Expression<Func<T, bool>> expression, SqlSelection sqlSelection)
        {
			return _resolve(expression, sqlSelection);
        }

		private LambdaResolutionResult _resolve<T>(Expression<Func<T, bool>> expression, SqlSelection sqlSelection)
		{
			var resolution = new SqlLam<T>(expression);
			var replaceString = "Select Top {0}";

			switch (sqlSelection)
			{
				case SqlSelection.All:
					return new LambdaResolutionResult(resolution.QueryString, resolution.QueryParameters);
				case SqlSelection.Top_1:
				case SqlSelection.Top_10:
				case SqlSelection.Top_100:
					return new LambdaResolutionResult(resolution.QueryString.ReplaceFirst("Select", string.Format(replaceString, (int)sqlSelection)), resolution.QueryParameters);
				default:
					return null;
			}
		}
    }
}
