using System;
using System.Data;
using System.Data.Common;
using CMSSolutions.Environment;

namespace CMSSolutions.Data.Entity
{
    public interface IDbContextFactory : IDisposable
    {
        DbContextBase GetContext<T>();
    }

    public class DbContextFactory : IDbContextFactory
    {
        private readonly IDbModelFactory dbModelFactory;
        private readonly IDataProviderFactory dataProviderFactory;
        private readonly DbConnection connection;

        public DbContextFactory(IDbModelFactory dbModelFactory, IDataProviderFactory dataProviderFactory, ShellSettings shellSettings)
        {
            this.dbModelFactory = dbModelFactory;
            this.dataProviderFactory = dataProviderFactory;
            connection = dataProviderFactory.CreateConnection(shellSettings.DataConnectionString);
        }

        public DbContextBase GetContext<T>()
        {
            var dbContext = new DefaultDbContext<T>(connection, dbModelFactory)
            {
                DataProviderFactory = dataProviderFactory
            };
            return dbContext;
        }

        public void Dispose()
        {
            if (connection != null && connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }
    }
}