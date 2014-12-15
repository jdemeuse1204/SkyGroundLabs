using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Enumeration;

namespace SkyGroundLabs.Data.Sql.Support
{
	public class InsertItem
	{
		public string Type { get; private set; }

		public string PropertyName { get; private set; }

		public string DatabaseColumnName { get; private set; }

		public string KeyName { get; private set; }

		public bool IsPrimaryKey { get; private set; }

		public DbGenerationType Generation { get; private set; }

		public object Value { get; private set; }

		public InsertItem(PropertyInfo property,object entity)
		{
			PropertyName = property.Name;
			DatabaseColumnName = property.GetDatabaseColumnName();
			IsPrimaryKey = property.IsPrimaryKey();
			Value = property.GetValue(entity);
			this.Type = Value.GetType().Name.ToUpper();
			Generation = property.GetDatabaseGenerationType();
			
			switch (Generation)
			{
				case DbGenerationType.None:
					KeyName = "";
					break;
				case DbGenerationType.IdentitySpecification:
					KeyName = "@@IDENTITY";
					break;
				case DbGenerationType.Generate:
					KeyName = string.Format("@{0}", PropertyName);
					// set as the property name so we can pull the value back out
					break;
			}

			switch (property.GetType().Name.ToUpper())
			{
				case "INT16":
					this.Type = "smallint";
					break;
				case "INT64":
					this.Type = "bigint";
					break;
				case "GUID":
					this.Type = "uniqueidentifier";
					break;
			}
		}
	}
}
