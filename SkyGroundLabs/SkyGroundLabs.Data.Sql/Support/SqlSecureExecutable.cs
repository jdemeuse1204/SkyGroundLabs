using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Support
{
	/// <summary>
	/// Uses parameters from SqlCommand to ensure safe sql commands are passed to the database
	/// </summary>
	public abstract class SqlSecureExecutable
	{
		#region Properties
		private Dictionary<string, object> _parameters { get; set; }
		#endregion

		#region Constructor
		public SqlSecureExecutable()
		{
			_parameters = new Dictionary<string, object>();
		}
		#endregion

		#region Methods
		protected string GetNextParameter()
		{
			return string.Format("@DATA{0}", _parameters.Count);
		}

		protected void AddParameter(object value)
		{
			_parameters.Add(GetNextParameter(), value);
		}

		protected void InsertParameters(SqlCommand cmd)
		{
			foreach (var item in _parameters)
			{
				cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = item.Key;
				cmd.Parameters[item.Key].Value = item.Value;
			}
		}
		#endregion
	}
}
