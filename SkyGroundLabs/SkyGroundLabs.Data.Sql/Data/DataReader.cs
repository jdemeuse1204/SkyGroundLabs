using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace SkyGroundLabs.Data.Sql.Data
{
	public class DataReader<T> : IEnumerable
	{
		private SqlDataReader _reader;

		public DataReader(SqlDataReader reader)
		{
			_reader = reader;
		}

		// IEnumerable Member
		public IEnumerator<T> GetEnumerator()
		{
			while (_reader.Read())
			{
				yield return _reader.ToObject<T>();
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			// Lets call the generic version here
			return this.GetEnumerator();
		}
	}
}
