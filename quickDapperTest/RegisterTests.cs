using quickDapper;
using System;
using Xunit;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

namespace quickDapperTest
{
    public class RegisterTests
    {
        private const string SqlString = @"";

        [Fact]
        public void RegisterTables()
        {
            var userReg = QuickDapper.RegisterTable<User>();
            var tripReg = QuickDapper.RegisterTable<Trip>();

            Assert.NotNull(userReg.QueryString);
            Assert.NotNull(userReg.InsertString);
            Assert.NotNull(userReg.UpdateString);

            Assert.NotNull(tripReg.QueryString);
            Assert.NotNull(tripReg.InsertString);
            Assert.NotNull(tripReg.UpdateString);
        }

        [Fact]
        public void RegisterPartial()
        {
            QuickDapper.RegisterPartial<UserCreateModel>();
        }

        //[Fact]
        //public async void Test1()
        //{


        //    Debug.Write("sss");

        //    using (IDbConnection dbConnection = new SqlConnection())
        //    {
        //        var duh = dbConnection.FindOne<User>(0);

        //        var updateModel = new TripUpdateModel()
        //        {
        //            City = string.Empty,
        //            Cost = 0,
        //            Country = string.Empty
        //        };

        //        var insertModel = new UserCreateModel()
        //        {

        //        };

        //        await dbConnection.UpdateAsync<Trip>(updateModel);
        //        await dbConnection.CreateAsync<User>(insertModel);
        //    }
        //}
    }
}
