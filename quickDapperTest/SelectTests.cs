using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using Xunit;
using quickDapperTest.Models;
using quickDapper;
using System.Data.SqlClient;

namespace quickDapperTest
{

    public class SelectTests
    {
        private const string SqlString = @"Data Source=hunchhunch.database.windows.net;Initial Catalog=hunchhunch;Persist Security Info=False;User ID=krieger;Password=ReadABook!;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False";

        [Fact]
        public async void SelectBike()
        {
            var connection = new SqlConnection(SqlString);

            var registerBike = QuickDapper.RegisterTable<Bike>();
            var registerMiniBike = QuickDapper.RegisterPartial<BikeMini>();
            Assert.NotNull(registerBike.QueryString);
            Assert.NotNull(registerMiniBike.QueryString);

            var bikes = await connection.GetAll<Bike>();
            var thundercat = await connection.FindOne<Bike>(1);
            var miniCat = await connection.FindOne<BikeMini>(1);
            Assert.NotNull(bikes);
            Assert.NotNull(thundercat);
            Assert.NotNull(miniCat);

            connection.Close();
        }
    }
}
