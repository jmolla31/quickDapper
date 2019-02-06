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
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            QuickDapper.RegisterTable<User>();
            QuickDapper.RegisterTable<Trip>();
            QuickDapper.RegisterPartial<UserCreateModel>();

            Debug.Write("sss");

            using (IDbConnection dbConnection = new SqlConnection())
            {
                var duh = dbConnection.FindOne<User>(0);

                var updateModel = new TripUpdateModel()
                {
                    City = string.Empty,
                    Cost = 0,
                    Country = string.Empty
                };

                var insertModel = new UserCreateModel()
                {

                };

                await dbConnection.UpdateAsync<Trip>(updateModel);
                await dbConnection.CreateAsync<User>(insertModel);
            }
        }
    }

    [TableName("Users")]
    public class User
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string Username { get; set; }
        [ServerField]
        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    [PartialModel(typeof(User))]
    public class UserCreateModel
    {
        public string Data { get; set; }
        public string Username { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class Trip
    {
        [PKey]
        public int TripId { get; set; }
        public double Cost { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }

    public class TripUpdateModel
    {
        public double Cost { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}
