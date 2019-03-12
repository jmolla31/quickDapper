using quickDapper;
using quickDapperTest.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Xunit;

namespace quickDapperTest
{
    public class CreateTests
    {
        private const string SqlString = @"Data Source=hunchhunch.database.windows.net;Initial Catalog=hunchhunch;Persist Security Info=False;User ID=krieger;Password=ReadABook!;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False";

        [Fact]
        public async void CreateBike()
        {
            var connection = new SqlConnection(SqlString);

            var registerBike = QuickDapper.RegisterTable<Bike>();
            Assert.NotNull(registerBike.QueryString);
            Assert.NotNull(registerBike.InsertString);

            var sampleBike = new Bike()
            {
                Model = "Z800",
                Manufacturer = "Kawasaki",
                Mileage = 0
            };

            var result = await connection.CreateAsync<Bike>(sampleBike);
            Assert.True(result > 0);

            connection.Close();
        }
    }
}
