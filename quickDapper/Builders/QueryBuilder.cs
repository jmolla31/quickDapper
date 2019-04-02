using System.Reflection;

namespace quickDapper.QueryBuilders
{
    internal static class QueryBuilder
    {
        /// <summary>
        /// Generates a SELECT statement with the provided values
        /// </summary>
        /// <param name="tableName">Table name in the database</param>
        /// <param name="properties">Properties (database columns) to be included in the statement</param>
        /// <returns></returns>
        internal static string BuildQuery(string tableName, PropertyInfo[] properties)
        {
            const string Separator = ", ";

            string sqlString = "SELECT ";

            foreach (var prop in properties)
            {
                sqlString += prop.Name;
                sqlString += Separator;
            }

            sqlString = sqlString.Substring(0, sqlString.Length - 2);
            sqlString += $" FROM {tableName}";

            return sqlString;
        }
    }
}
