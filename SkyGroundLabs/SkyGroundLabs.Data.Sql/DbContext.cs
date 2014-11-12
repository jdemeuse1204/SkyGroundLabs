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
					set += string.Format("[{0}] = {1},", item.Key, paramName);
					updateValues.Add(paramName, item.Value);
					count++;
				}

				sql += set.TrimEnd(',');
				var validationValues = new Dictionary<string, string>();
				foreach (var item in update.GetValidation())
				{
					var paramName = string.Format("@Data{0}", count);
					validation += string.Format("[{0}] = {1},", item.Key, paramName);
					validationValues.Add(paramName, item.Value);
					count++;
				}

				sql += string.Format(" WHERE {0}", validation.TrimEnd(','));
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
					fields += string.Format("[{0}],", item.Key);
					values += paramName + ",";
					insertValues.Add(paramName, item.Value);
					count++;
				}

				sql += string.Format("({0}) VALUES ", fields.TrimEnd(','));
				sql += string.Format("({0});Select @@IDENTITY;", values.TrimEnd(','));

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

		private string _getSelectSql(QuerySelect select, out Dictionary<string, string> validationValues)
		{
			validationValues = new Dictionary<string, string>();
			var sql = "SELECT TOP 1 {0}";
			var from = string.Format(" FROM {0},", select.Table);
			var fields = string.Empty;
			var validation = string.Empty;


			if (select.Count == 0)
			{
				fields += "*";
			}
			else
			{
				foreach (var item in select)
				{
					fields += string.Format("[{0}].[{1}],", select.Table, item.Key);
				}
			}

			var joins = select.GetJoins();
			var joinCount = 0;
			foreach (var item in joins)
			{
				validation += string.Format((joinCount == 0 ? " WHERE " : " AND ") + "[{0}].[{1}] = [{2}].[{3}]",
					item.ParentTable,
					item.ParentTableJoinValue,
					item.ChildTable,
					item.ChildTableJoinValue);

				from += string.Format("{0},", item.ChildTable);

				foreach (var field in item)
				{
					fields += string.Format("[{0}].[{1}],", item.ChildTable, field);
				}
				joinCount++;
			}

			sql = string.Format(sql, fields.TrimEnd(','));
			sql += from.TrimEnd(',');

			var count = 0;
			foreach (var item in select.GetValidation())
			{
				var paramName = string.Format("@Data{0}", count);
				validation += string.Format((!validation.Contains("WHERE") ? " WHERE " : " AND ") + "[{0}].[{1}] = {2}", select.Table, item.Key, paramName);
				validationValues.Add(paramName, item.Value);
				count++;
			}

			foreach (var item in joins)
			{
				foreach (var field in item.GetValidation())
				{
					var paramName = string.Format("@Data{0}", count);
					validation += string.Format((!validation.Contains("WHERE") ? " WHERE " : " AND ") + "[{0}].[{1}] = {2}", item.ChildTable, field.Key, paramName);
					validationValues.Add(paramName, field.Value);
					count++;
				}
			}

			sql += validation;
			return sql;
		}

		public T Select<T>(QuerySelect select) where T : class
		{
			var validationValues = new Dictionary<string, string>();
			var sql = _getSelectSql(select, out validationValues);

			using (SqlConnection con = new SqlConnection(_sqlConnection))
			{
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					foreach (var item in validationValues)
					{
						cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = item.Key;
						cmd.Parameters[item.Key].Value = item.Value;
					}

					if (con.State == ConnectionState.Closed)
					{
						con.Open();
					}

					var reader = cmd.ExecuteReader();

					reader.Read();

					return reader.ToObject<T>();
				}
			}
		}

		public dynamic Select(QuerySelect select)
		{
			var validationValues = new Dictionary<string, string>(); 
			var sql = _getSelectSql(select,out validationValues);

			using (SqlConnection con = new SqlConnection(_sqlConnection))
			{
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					foreach (var item in validationValues)
					{
						cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = item.Key;
						cmd.Parameters[item.Key].Value = item.Value;
					}

					if (con.State == ConnectionState.Closed)
					{
						con.Open();
					}

					var reader = cmd.ExecuteReader();

					reader.Read();

					return reader.ToObject();
				}
			}
		}
	}
}
