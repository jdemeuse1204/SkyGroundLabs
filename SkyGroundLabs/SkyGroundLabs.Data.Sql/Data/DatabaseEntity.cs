using System;
using System.Collections.Generic;
using System.Reflection;
using SkyGroundLabs.Data.Sql.Mapping;

namespace SkyGroundLabs.Data.Sql.Data
{
    public static class DatabaseEntity
    {
        public static ModificationState GetState(object entity, List<PropertyInfo> primaryKeys)
        {
            for (var i = 0; i < primaryKeys.Count; i++)
            {
                var key = primaryKeys[i];
                var pkValue = key.GetValue(entity);
                var generationOption = DatabaseSchemata.GetGenerationOption(key);
                var isUpdating = false;

                if (generationOption != DbGenerationOption.None)
                {
                    // If Db generation option is set to none, we always do an insert

                    switch (pkValue.GetType().Name.ToUpper())
                    {
                        case "INT16":
                            isUpdating = Convert.ToInt16(pkValue) != 0;
                            break;
                        case "INT32":
                            isUpdating = Convert.ToInt32(pkValue) != 0;
                            break;
                        case "INT64":
                            isUpdating = Convert.ToInt64(pkValue) != 0;
                            break;
                        case "GUID":
                            isUpdating = (Guid) pkValue != Guid.Empty;
                            break;
                    }
                }

                // break because we are already updating, do not want to set to false
                if (!isUpdating)
                {
                    continue;
                }

                return ModificationState.Update;
            }

            return ModificationState.Insert;
        }
    }
}
