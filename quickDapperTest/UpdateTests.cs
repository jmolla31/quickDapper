using quickDapper;
using quickDapperTest.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Xunit;

namespace quickDapperTest
{
    public class UpdateTests
    {
        [Fact]
        public async void UpdateBike()
        {
            var connection = new SqlConnection(Secrets.SqlString);

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

            var trashedZ = new UglyTrashedZ() //Really, who likes those bikes?
            {
                BikeNumber = 3,
                Mileage = 99999999
            };

            var update = await connection.UpdatePartialAsync<UglyTrashedZ>(trashedZ);
            Assert.True(update > 0);

            connection.Close();
        }
    }
}
