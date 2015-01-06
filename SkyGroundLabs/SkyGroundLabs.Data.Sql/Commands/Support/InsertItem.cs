using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Sql.Enumeration;
using SkyGroundLabs.Data.Sql.Mapping;

namespace SkyGroundLabs.Data.Sql.Commands.Support
{
	public class InsertItem
	{
		public string SqlDataTypeString { get; private set; }

		public string PropertyDataType { get; private set; }

		public string PropertyName { get; private set; }

		public string DatabaseColumnName { get; private set; }

		public string KeyName { get; private set; }

		public SqlDbType DbTranslationType { get; private set; }

		public bool IsPrimaryKey { get; private set; }

		public DbGenerationType Generation { get; private set; }

		public object Value { get; private set; }

		public bool TranslateDataType { get; private set; }

		public InsertItem(PropertyInfo property,object entity)
		{
			PropertyName = property.Name;
			DatabaseColumnName = property.GetDatabaseColumnName();
			IsPrimaryKey = property.IsPrimaryKey();
			Value = property.GetValue(entity);
			PropertyDataType = property.PropertyType.Name.ToUpper();
			Generation = property.GetDatabaseGenerationType();

			// check for sql data translation, used mostly for datetime2 inserts and updates
			var translation = property.GetCustomAttribute<DbTranslationAttribute>();

			if (translation != null)
			{
				DbTranslationType = translation.Type;
				TranslateDataType = true;
			}

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

			// for auto generation
			switch (property.PropertyType.Name.ToUpper())
			{
				case "INT16":
					this.SqlDataTypeString = "smallint";
					break;
				case "INT64":
					this.SqlDataTypeString = "bigint";
					break;
				case "INT32":
					this.SqlDataTypeString = "int";
					break;
				case "GUID":
					this.SqlDataTypeString = "uniqueidentifier";
					break;
			}
		}
	}
}
