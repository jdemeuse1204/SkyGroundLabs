using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Commands;
using SkyGroundLabs.Data.Sql.Support;

namespace SkyGroundLabs.Data.Sql
{
	public class DbContext : IDisposable
	{
		private SqlConnection _connection { get; set; }
		private SqlCommand _cmd { get; set; }
		private SqlDataReader _reader { get; set; }

		public DbContext(string connectionString)
		{
			_connection = new SqlConnection(connectionString);
		}

		public void Disconnect()
		{
			_connection.Close();
		}

		public void ExecuteSql(ISqlBuilder builder)
		{
			_cmd = builder.BuildCommand(_connection);

			_connect();
			_reader = _cmd.ExecuteReader();
		}

		public T First<T>()
		{
			_reader.Read();
			return _reader.ToObject<T>();
		}

		public dynamic First()
		{
			_reader.Read();
			return _reader.ToObject();
		}

		public dynamic Select()
		{
			return _reader.ToObject();
		}

		public T Select<T>()
		{
			return _reader.ToObject<T>();
		}

		public bool HasNext()
		{
			return _reader.Read();
		}

		public List<T> SelectAll<T>()
		{
			var result = new List<T>();

			while (_reader.Read())
			{
				result.Add(_reader.ToObject<T>());
			}

			return result;
		}

		public List<dynamic> SelectAll()
		{
			var result = new List<dynamic>();

			while (_reader.Read())
			{
				result.Add(_reader.ToObject());
			}

			return result;
		}

		public void Dispose()
		{
			_cmd.Dispose();
			_connection.Close();
			_connection.Dispose();
		}

		private void _connect()
		{
			if (_connection.State == ConnectionState.Closed)
			{
				_connection.Open();
			}
		}
	}
}
