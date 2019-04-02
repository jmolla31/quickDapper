using Dapper;
using quickDapper.QueryBuilders;
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
        private static readonly IDictionary<Type, TableObject> MainTableCache = new Dictionary<Type, TableObject>();
        private static readonly IDictionary<Type, PartialTableObject> PartialTableCache = new Dictionary<Type, PartialTableObject>();

        /// <summary>
        /// Register a new table in the main table cache.
        /// </summary>
        /// <typeparam name="T">Class mapping the table</typeparam>
        /// <param name="appendS">Enable/Disable appending an 's' char at the end when generating the table name</param>
        /// <param name="query">Enable/Disable SELECT statement generation</param>
        /// <param name="insert">Enable/Disable INSERT statement generation</param>
        /// <param name="update">Enable/Disable UPDATE statement generation</param>
        /// <returns>Returns a the generated TableObject that can be stored to check the generated values but normally it should be ignored.</returns>
        public static TableObject RegisterTable<T>(bool appendS = true, bool query = true, bool insert = true, bool update = true) where T : class
        {
            var type = typeof(T);
            var propList = type.GetProperties();
            var attrList = type.CustomAttributes;

            var tableNameAttr = attrList.FirstOrDefault(x => x.AttributeType.Name == "TableNameAttribute");
            var tableName = (tableNameAttr != null) ? tableNameAttr.ConstructorArguments[0].Value as string : type.Name;

            if (appendS) tableName += "s";

            var pKeyAttr = propList.FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(PKeyAttribute)));
            var pKey = (pKeyAttr != null) ? pKeyAttr.Name : propList.Single(prop => prop.Name == "Id").Name;


            var tableObject = new TableObject()
            {
                Type = type,
                TableName = tableName,
                PrimaryKey = pKey,
                QueryString = (query) ? QueryBuilder.BuildQuery(tableName, propList) : null,
                InsertString = (insert) ? InsertBuilder.BuildQuery(tableName, pKey, propList) : null,
                UpdateString = (update) ? UpdateBuilder.BuildQuery(tableName, pKey, propList) : null
            };

            return (MainTableCache.TryAdd(type, tableObject)) ? tableObject : throw new Exception("Error adding tableObject to dictionary");
        }

        /// <summary>
        /// Registers a new partial table that references a full TableObject registered earlier.
        /// </summary>
        /// <typeparam name="T">Class partially mapping the table.</typeparam>
        /// <param name="query">Enable/Disable SELECT statement generation</param>
        /// <param name="insert">Enable/Disable INSERT statement generation</param>
        /// <param name="update">Enable/Disable UPDATE statement generation</param>
        /// <returns></returns>
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
                QueryString = (query) ? QueryBuilder.BuildQuery(mainTable.TableName, propList) : null,
                InsertString = (insert) ? InsertBuilder.BuildQuery(mainTable.TableName, mainTable.PrimaryKey, propList) : null,
                UpdateString = (update) ? UpdateBuilder.BuildQuery(mainTable.TableName, mainTable.PrimaryKey, propList) : null
            };

            return (PartialTableCache.TryAdd(type, partialTable)) ? partialTable : throw new Exception("Error adding partialTableObject to dictionary");
        }


        /// <summary>
        /// Finds one entity filtered by primary key using the generated SELECT statement<para/>
        /// Returns the result mapped to the provided class
        /// </summary>
        /// <typeparam name="Entity">Entity that's going to be queried</typeparam>
        /// <param name="dbConn">Database Connection</param>
        /// <param name="primaryKey">Primary key value to be queried</param>
        /// <returns></returns>
        public static async Task<Entity> FindOne<Entity>(this IDbConnection dbConn, dynamic primaryKey) where Entity : class
        {
            var isCached = MainTableCache.TryGetValue(typeof(Entity), out TableObject cachedTable);
            if (!isCached) throw new KeyNotFoundException($"Error, key {typeof(Entity)} not found in table cache.");

            var sqlString = $"{cachedTable.QueryString} WHERE {cachedTable.PrimaryKey} = @PrimaryKey";

            var queryParams = new { PrimaryKey = primaryKey };

            var result = await dbConn.QueryFirstOrDefaultAsync<Entity>(sqlString, queryParams);

            return result;
        }

        /// <summary>
        /// Finds all rows of an entity using the generated SELECT statement<para/>
        /// Returns the result mapped to an IEnumerable of the provided class
        /// </summary>
        /// <typeparam name="Entity">Entity that's going to be queried</typeparam>
        /// <param name="dbConn">Database Connection</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Entity>> GetAll<Entity>(this IDbConnection dbConn) where Entity : class
        {
            var isCached = MainTableCache.TryGetValue(typeof(Entity), out TableObject cachedTable);
            if (!isCached) throw new KeyNotFoundException($"Error, key {typeof(Entity)} not found in table cache.");

            var sqlString = cachedTable.QueryString;

            var result = await dbConn.QueryAsync<Entity>(sqlString);

            return result;
        }

        /// <summary>
        /// Inserts a new entry in the databse using the generated INSERT statement<para/>
        /// Returns the number of rows affected by the statement
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="dbConn">Database connection</param>
        /// <param name="insertModel">Class containing the data to insert</param>
        /// <returns></returns>
        public static async Task<int> CreateAsync<Entity>(this IDbConnection dbConn, object insertModel) where Entity : class
        {
            //TODO:  Check it's not a partial, mark models with some key???
            var isCached = MainTableCache.TryGetValue(typeof(Entity), out TableObject cachedTable);
            if (!isCached) throw new KeyNotFoundException($"Error, key {typeof(Entity)} not found in table cache.");

            var sqlString = cachedTable.InsertString;

            //TODO: Return identity of last insert instead of rows affected. If possible, add optional configuration bool to choose between the options
            var result = await dbConn.ExecuteAsync(sqlString, insertModel);

            return result;
        }

        /// <summary>
        /// Updates an entry in the database using the generated UPDATE statement<para/>
        /// Returns the number of rows affected by the statement
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="updateModel">Class containing the data to insert</param>
        /// <returns></returns>
        public static async Task<int> UpdateAsync<Entity>(this IDbConnection dbConn, object updateModel) where Entity : class
        {
            //TODO:  Check it's not a partial, mark models with some key???
            var isCached = MainTableCache.TryGetValue(typeof(Entity), out TableObject cachedTable);
            if (!isCached) throw new KeyNotFoundException($"Error, key {typeof(Entity)} not found in table cache.");

            var sqlString = cachedTable.UpdateString;

            var result = await dbConn.ExecuteAsync(sqlString, updateModel);

            return result;
        }

        /// <summary>
        /// Updates an entry in the database using a partial model instead of the full TableObject<para/>
        /// Returns the number of rows affected by the statement
        /// </summary>
        /// <typeparam name="Partial"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        public static async Task<int> UpdatePartialAsync<Partial>(this IDbConnection dbConn, object updateModel) where Partial : class
        {
            var isCached = PartialTableCache.TryGetValue(typeof(Partial), out PartialTableObject cachedPartial);
            if (!isCached) throw new KeyNotFoundException($"Error, key {typeof(Partial)} not found in partial table cache.");

            var sqlString = cachedPartial.UpdateString;

            var result = await dbConn.ExecuteAsync(sqlString, updateModel);

            return result;
        }
    }
}

