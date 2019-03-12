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
        private const string SqlString = @"Data Source=hunchhunch.database.windows.net;Initial Catalog=hunchhunch;Persist Security Info=False;User ID=krieger;Password=ReadABook!;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False";

        [Fact]
        public async void TestSqlConnection()
        {
            var connection = new SqlConnection(SqlString);

            await connection.OpenAsync();
            var sVersion = connection.ServerVersion;

            Assert.NotNull(sVersion);

            connection.Close();
            connection.Dispose();
        }
    }
}
