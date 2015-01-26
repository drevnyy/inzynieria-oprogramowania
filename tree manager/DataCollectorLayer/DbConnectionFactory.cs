using System;
using System.Data.Common;

namespace DataCollectorLayer
{

    public class DbConnectionFactory
    {
        public DbConnection CreateDbConnection(string driver, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(driver))
                throw new NullReferenceException("connectionString or driver is empty");

            DbProviderFactory factory =
                DbProviderFactories.GetFactory(driver);

            DbConnection connection = factory.CreateConnection();
            if (connection != null) connection.ConnectionString = connectionString;
            return connection;

        }

    }
}
