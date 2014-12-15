using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Support;
using SkyGroundLabs.Data.Sql.Enumeration;
using System.Reflection;
using SkyGroundLabs.Data.Sql.KeyGeneration;
using SkyGroundLabs.Data.Sql.Mapping;

namespace SkyGroundLabs.Data.Sql.Commands
{
	public class SqlInsertBuilder : SqlSecureExecutable, ISqlBuilder
	{
		#region Properties
		private string _table { get; set; }
		private List<InsertItem> _insertItems { get; set; }
		#endregion

		#region Constructor
		public SqlInsertBuilder()
			: base()
		{
			_table = string.Empty;
			_insertItems = new List<InsertItem>();
		}
		#endregion

		#region Methods
		public SqlCommand BuildCommand(SqlConnection connection)
		{
			if (string.IsNullOrWhiteSpace(_table))
			{
				throw new QueryNotValidException("INSERT statement needs Table Name");
			}

			if (_insertItems.Count == 0)
			{
				throw new QueryNotValidException("INSERT statement needs VALUES");
			}

			var fields = string.Empty;
			var values = string.Empty;
			var identity = string.Empty;
			var declare = string.Empty;
			var set = string.Empty;

			//  NOTE:  Alias any Identity specification and generate columns with their property
			// name not db column name so we can set the property when we return the values back.

			foreach (var item in _insertItems)
			{
				if (item.Generation == DbGenerationType.None && !item.IsPrimaryKey)
				{
					//Value is simply inserted
					var data = GetNextParameter();
					fields += string.Format("[{0}],", item.DatabaseColumnName);
					values += string.Format("{0},", data);
					AddParameter(item.Value);
				}
				else if (item.Generation == DbGenerationType.Generate)
				{
					// Value is generated from the database
					var key = string.Format("@{0}", item.PropertyName);

					// alias as the property name so we can set the property
					var variable = string.Format("{0} as {1}", key, item.PropertyName);

					// make our set statement
					if (item.SqlDataType.ToUpper() == "UNIQUEIDENTIFIER")
					{
						// GUID
						set += string.Format("SET {0} = NEWID();", key);
					}
					else
					{
						// INTEGER
						set += string.Format("SET {0} = (Select ISNULL(MAX({1}),0) + 1 From {2});", key, item.DatabaseColumnName, _table);
					}

					fields += string.Format("[{0}],", item.DatabaseColumnName);
					values += string.Format("{0},", key);
					declare += string.Format("{0} as {1},", key, item.SqlDataType);
					identity += variable + ",";

					// Do not add as a parameter because the parameter will be converted to a string to
					// be inserted in to the database
				}
				else 
				{
					// We assume the database will generate our value
					identity = string.Format("@@IDENTITY as {0},", item.PropertyName);
				}
			}

			var sql = string.Format("{0} {1} INSERT INTO {2} ({3}) VALUES ({4});{5}",
				string.IsNullOrWhiteSpace(declare) ? "" : string.Format("DECLARE {0}", declare.TrimEnd(',')),
				set,
				_table, fields.TrimEnd(','),
				values.TrimEnd(','),
				string.IsNullOrWhiteSpace(identity) ? "" : string.Format("Select {0}", identity.TrimEnd(',')));

			var cmd = new SqlCommand(sql, connection);

			InsertParameters(cmd);

			return cmd;
		}

		public void Table(string tableName)
		{
			_table = tableName;
		}

		public void AddInsert(PropertyInfo property, object entity)
		{
			_insertItems.Add(new InsertItem(property, entity));
		}
		#endregion
	}
}
