using Dapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace quickDapper
{
    public static class QuickDapper
    {
        //TODO: Remove "runtime" registering in methods and change to a normal Dictionary, forcing developers to register all tables at startup time.
        private static readonly ConcurrentDictionary<Type, TableObject> MainTableCache = new ConcurrentDictionary<Type, TableObject>();
        private static readonly ConcurrentDictionary<Type, PartialTableObject> PartialTableCache = new ConcurrentDictionary<Type, PartialTableObject>();

        public static TableObject RegisterTable<T>(bool query = true, bool insert = true, bool update = true) where T : class
        {
            var type = typeof(T);
            var propList = type.GetProperties();
            var attrList = type.CustomAttributes;

            var tableNameAttr = attrList.FirstOrDefault(x => x.AttributeType.Name == "TableNameAttribute");
            var tableName = (tableNameAttr != null) ? tableNameAttr.ConstructorArguments[0].Value as string : type.Name;

            var pKeyAttr = propList.FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(PKeyAttribute)));
            var pKey = (pKeyAttr != null) ? pKeyAttr.Name : propList.Single(prop => prop.Name == "Id").Name;


            var tableObject = new TableObject()
            {
                Type = type,
                TableName = tableName,
                PrimaryKey = pKey,
                QueryString = (query) ? QueryConstructor(tableName, propList) : null,
                InsertString = (insert) ? InsertConstructor(tableName, pKey, propList) : null,
                UpdateString = (update) ? UpdateConstructor(tableName, pKey, propList) : null
            };

            return (MainTableCache.TryAdd(type, tableObject)) ? tableObject : throw new Exception("Error adding tableObject to dictionary");
        }

        public static PartialTableObject RegisterPartial<T>(bool query = true, bool insert = true, bool update = true) where T : class
        {
            var type = typeof(T);
            var propList = type.GetProperties();
            var attrList = type.CustomAttributes;

            var mainTableAttr = attrList.FirstOrDefault(x => x.AttributeType.Name == "PartialModelAttribute") ?? throw new Exception("Attribute not found");
            var referencedTable = mainTableAttr.ConstructorArguments[0].Value as Type ?? throw new Exception("Can't cast attribute to Type");

            var fetchMainTable = MainTableCache.TryGetValue(referencedTable, out TableObject mainTable);

            if (!fetchMainTable) throw new Exception("Can't find main table in table cache.");

            var partialTable = new PartialTableObject()
            {
                MainTable = referencedTable,
                QueryString = (query) ? QueryConstructor(mainTable.TableName, propList) : null,
                InsertString = (insert) ? InsertConstructor(mainTable.TableName, mainTable.PrimaryKey, propList) : null,
                UpdateString = (update) ? UpdateConstructor(mainTable.TableName, mainTable.PrimaryKey, propList) : null
            };

            return (PartialTableCache.TryAdd(type, partialTable)) ? partialTable : throw new Exception("Error adding partialTableObject to dictionary");
        }

        private static string QueryConstructor(string tableName, PropertyInfo[] properties)
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

        private static string UpdateConstructor(string tableName, string pKey, PropertyInfo[] properties)
        {
            const string Separator = ", ";

            string sqlString = $"UPDATE {tableName} SET ";

            foreach (var prop in properties)
            {
                if (!Attribute.IsDefined(prop, typeof(ServerFieldAttribute)) && prop.Name != pKey)
                {
                    sqlString += $"{prop.Name} = @{prop.Name}";
                    sqlString += Separator;
                }
            }
            sqlString = sqlString.Substring(0, sqlString.Length - 2);
            sqlString += $" WHERE {pKey} = @{pKey}";
            return sqlString;
        }

        private static string InsertConstructor(string tableName, string pKey, PropertyInfo[] properties)
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

        public static async Task<Entity> FindOne<Entity>(this IDbConnection dbConn, dynamic primaryKey) where Entity : class
        {
            var isCached = MainTableCache.TryGetValue(typeof(Entity), out TableObject cachedTable);
            if (!isCached) cachedTable = RegisterTable<Entity>();

            var sqlString = $"{cachedTable.QueryString} WHERE {cachedTable.PrimaryKey} = @PrimaryKey";

            var queryParams = new { PrimaryKey = primaryKey };

            var result = await dbConn.QueryFirstOrDefaultAsync<Entity>(sqlString, queryParams);

            return result;
        }

        public static async Task<IEnumerable<Entity>> GetAll<Entity>(this IDbConnection dbConn) where Entity : class
        {
            var isCached = MainTableCache.TryGetValue(typeof(Entity), out TableObject cachedTable);
            if (!isCached) cachedTable = RegisterTable<Entity>();

            var sqlString = cachedTable.QueryString;

            var result = await dbConn.QueryAsync<Entity>(sqlString);

            return result;
        }

        public static async Task<int> CreateAsync<Entity>(this IDbConnection dbConn, object insertModel) where Entity : class
        {
            //TODO:  Check it's not a partial, mark models with some key???
            var isCached = MainTableCache.TryGetValue(typeof(Entity), out TableObject cachedTable);
            if (!isCached) cachedTable = RegisterTable<Entity>();

            var sqlString = cachedTable.InsertString;

            var result = await dbConn.ExecuteAsync(sqlString, insertModel);

            return result;
        }

        public static async Task<int> UpdateAsync<Entity>(this IDbConnection dbConn, object updateModel) where Entity : class
        {
            //TODO:  Check it's not a partial, mark models with some key???
            var isCached = MainTableCache.TryGetValue(typeof(Entity), out TableObject cachedTable);
            if (!isCached) cachedTable = RegisterTable<Entity>();

            var sqlString = cachedTable.UpdateString;

            var result = await dbConn.ExecuteAsync(sqlString, updateModel);

            return result;
        }

        public static async Task<int> PartialUpdateAsync<Partial>(this IDbConnection dbConn, object updateModel) where Partial : class
        {
            var isCached = PartialTableCache.TryGetValue(typeof(Partial), out PartialTableObject cachedPartial);
            if (!isCached) cachedPartial = RegisterPartial<Partial>();

            var sqlString = cachedPartial.UpdateString;

            var result = await dbConn.ExecuteAsync(sqlString, updateModel);

            return result;
        }
    }
}

