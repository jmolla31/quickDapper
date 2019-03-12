using quickDapper;
using System;
using Xunit;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using quickDapperTest.Models;

namespace quickDapperTest
{
    public class RegisterTests
    {
        private const string SqlString = @"";

        [Fact]
        public void RegisterTables()
        {
            var registerBike = QuickDapper.RegisterTable<Bike>();
            Assert.NotNull(registerBike.QueryString);
            Assert.NotNull(registerBike.InsertString);
            Assert.NotNull(registerBike.UpdateString);

            var registerEvent = QuickDapper.RegisterTable<Event>();
            Assert.NotNull(registerEvent.QueryString);
            Assert.NotNull(registerEvent.InsertString);
            Assert.NotNull(registerEvent.UpdateString);

            var registerTrack = QuickDapper.RegisterTable<Track>();
            Assert.NotNull(registerTrack.QueryString);
            Assert.NotNull(registerTrack.InsertString);
            Assert.NotNull(registerTrack.UpdateString);
        }

        [Fact]
        public void RegisterPartial()
        {
            var registerBike = QuickDapper.RegisterTable<Bike>();
            Assert.NotNull(registerBike.QueryString);
            Assert.NotNull(registerBike.InsertString);
            Assert.NotNull(registerBike.UpdateString);

            var registerMiniBike = QuickDapper.RegisterPartial<BikeMini>();
            Assert.NotNull(registerMiniBike.QueryString);
            Assert.NotNull(registerMiniBike.InsertString);
            Assert.NotNull(registerMiniBike.UpdateString);
        }
    }
}
