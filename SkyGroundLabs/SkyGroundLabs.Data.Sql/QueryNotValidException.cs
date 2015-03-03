using System;

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
