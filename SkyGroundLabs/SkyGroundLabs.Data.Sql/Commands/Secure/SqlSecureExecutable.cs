using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SkyGroundLabs.Data.Sql.Commands.Secure
{
	/// <summary>
	/// Uses parameters from SqlCommand to ensure safe sql commands are passed to the database
	/// </summary>
	public abstract class SqlSecureExecutable
	{
		#region Properties
		private Dictionary<string, SqlSecureObject> _parameters { get; set; }
		#endregion

		#region Constructor
	    protected SqlSecureExecutable()
		{
			_parameters = new Dictionary<string, SqlSecureObject>();
		}
		#endregion

		#region Methods
		protected string GetNextParameter()
		{
			return string.Format("@DATA{0}", _parameters.Count);
		}

		protected void AddParameter(object parameterValue)
		{
			_parameters.Add(GetNextParameter(), new SqlSecureObject(parameterValue));
		}

		protected void AddParameter(string parameterKey, object parameterValue)
		{
			_parameters.Add(parameterKey, new SqlSecureObject(parameterValue));
		}

		protected void AddParameter(object parameterValue, SqlDbType type)
		{
			_parameters.Add(GetNextParameter(), new SqlSecureObject(parameterValue, type));
		}

		protected void AddParameter(string parameterKey, object parameterValue, SqlDbType type)
		{
			_parameters.Add(parameterKey, new SqlSecureObject(parameterValue, type));
		}

		protected void InsertParameters(SqlCommand cmd)
		{
			foreach (var item in _parameters)
			{
				cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = item.Key;
				cmd.Parameters[item.Key].Value = item.Value.Value;

				if (item.Value.TranslateDataType)
				{
					cmd.Parameters[item.Key].SqlDbType = item.Value.DbTranslationType;
				}
			}
		}
		#endregion
	}
}
