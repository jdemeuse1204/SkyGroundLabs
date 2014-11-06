using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Commands;

namespace SkyGroundLabs.Data.Sql
{
	public class DbContext
	{
		private string _sqlConnection { get; set; }

		public DbContext(string sqlConnection)
		{
			_sqlConnection = sqlConnection;
		}

		public void Update(QueryUpdate update)
		{
			var sql = string.Format("UPDATE {0} SET ", update.Table);
			var set = string.Empty;
			var validation = string.Empty;

			using (SqlConnection con = new SqlConnection(_sqlConnection))
			{
				var count = 0;
				var updateValues = new Dictionary<string, string>();
				foreach (var item in update)
				{
					var paramName = string.Format("@Data{0}", count);
					set += "[" + item.Key + "] = " + paramName + ",";
					updateValues.Add(paramName, item.Value);
					count++;
				}

				sql += set.TrimEnd(',');
				var validationValues = new Dictionary<string, string>();
				foreach (var item in update.GetValidation())
				{
					var paramName = string.Format("@Data{0}", count);
					validation += "[" + item.Key + "] = " + paramName + ",";
					validationValues.Add(paramName, item.Value);
					count++;
				}

				sql += " WHERE " + validation.TrimEnd(',');
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					foreach (var item in updateValues)
					{
						cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = item.Key;
						cmd.Parameters[item.Key].Value = item.Value;
					}

					foreach (var item in validationValues)
					{
						cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = item.Key;
						cmd.Parameters[item.Key].Value = item.Value;
					}

					if (con.State == ConnectionState.Closed)
					{
						con.Open();
					}

					cmd.ExecuteReader();
				}
			}
		}

		public object Insert(QueryInsert insert)
		{
			var sql = string.Format("INSERT INTO {0}", insert.Table);
			var fields = string.Empty;
			var values = string.Empty;
			var key = new object();

			using (SqlConnection con = new SqlConnection(_sqlConnection))
			{
				var count = 0;
				var insertValues = new Dictionary<string, string>();
				foreach (var item in insert)
				{
					var paramName = string.Format("@Data{0}", count);
					fields += "[" + item.Key + "]" + ",";
					values += paramName + ",";
					insertValues.Add(paramName, item.Value);
					count++;
				}

				sql += " (" + fields.TrimEnd(',') + ") VALUES ";
				sql += " (" + values.TrimEnd(',') + ");Select @@IDENTITY;";

				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					foreach (var item in insertValues)
					{
						cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = item.Key;
						cmd.Parameters[item.Key].Value = item.Value;
					}

					if (con.State == ConnectionState.Closed)
					{
						con.Open();
					}

					SqlDataReader reader = cmd.ExecuteReader();
					reader.Read();
					return reader.GetValue(0);
				}
			}
		}

		public dynamic Select(QuerySelect select)
		{
			var sql = "SELECT ";
			var fields = string.Empty;
			var validation = string.Empty;

			using (SqlConnection con = new SqlConnection(_sqlConnection))
			{
				foreach (var item in select)
				{
					var functions = string.Empty;
					if (item.Value != null && item.Value.Count > 0)
					{
						foreach (var function in item.Value)
						{
							if (string.IsNullOrWhiteSpace(functions))
							{
								functions = function;
							}
							else
							{
								// nest the functions
								functions = string.Format(functions, function);
							}
						}
					}

					if (string.IsNullOrWhiteSpace(functions))
					{
						fields += "[" + string.Format(functions,item.Key) + "],";
					}
					else
					{
						fields += "[" + item.Key + "],";
					}
				}
			}

			return null;
		}
	}
}
