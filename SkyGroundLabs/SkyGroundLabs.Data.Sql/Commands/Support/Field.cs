using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Commands.Support
{
	public class Field
	{
		public string ColumnName { get; set; }
		public string Alias { get; set; }

		public Field(string columnName, string alias = "")
		{
			ColumnName = columnName;
			Alias = alias;
		}
	}
}
