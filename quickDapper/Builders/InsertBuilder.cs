using System;
using System.Reflection;

namespace quickDapper.QueryBuilders
{
    internal static class InsertBuilder
    {
        /// <summary>
        /// Generates a INSERT statement for the provided values
        /// doesn't include the primary key and fields marked with the ServerField attribute
        /// </summary>
        /// <param name="tableName">Table name in the database</param>
        /// <param name="pKey">PrimaryKey column</param>
        /// <param name="properties">Properties (database columns) to be included in the statement</param>
        /// <returns></returns>
        internal static string BuildQuery(string tableName, string pKey, PropertyInfo[] properties)
        {
            const string Separator = ", ";

            string sqlString = $"INSERT INTO {tableName} VALUES (";

            foreach (var prop in properties)
            {
                if (!Attribute.IsDefined(prop, typeof(ServerFieldAttribute)) && prop.Name != pKey)
                {
                    sqlString += $"@{prop.Name}";
                    sqlString += Separator;
                }
            }
            sqlString = sqlString.Substring(0, sqlString.Length - 2);
            sqlString += ")";

            return sqlString;
        }
    }
}
