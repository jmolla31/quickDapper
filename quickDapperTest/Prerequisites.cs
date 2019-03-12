using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Xunit;

namespace quickDapperTest
{
    public class Prerequisites
    {

        [Fact]
        public async void TestSqlConnection()
        {
            var connection = new SqlConnection(Secrets.SqlString);

            await connection.OpenAsync();
            var sVersion = connection.ServerVersion;

            Assert.NotNull(sVersion);

            connection.Close();
            connection.Dispose();
        }
    }
}
