using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Data.Entity
{
    public interface IDataProviderFactory : IDependency
    {
        DbConnection CreateConnection(string dataConnectionString);

        void EnsureTables<TContext>(TContext context) where TContext : DbContextBase;
    }

    [Feature(Constants.Areas.Core)]
    public class SqlServerDataProviderFactory : IDataProviderFactory
    {
        public DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnectionFactory().CreateConnection(connectionString);
        }

        public void EnsureTables<TContext>(TContext context) where TContext : DbContextBase
        {
            var script = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
            if (!string.IsNullOrEmpty(script))
            {
                List<string> existingTableNames;
                try
                {
                    existingTableNames = new List<string>();
                    var tables = context.Database.SqlQuery(typeof(string), "SELECT table_name from INFORMATION_SCHEMA.TABLES WHERE table_type = 'base table'");
                    var tablesEnumerator = tables.GetEnumerator();

                    while (tablesEnumerator.MoveNext())
                    {
                        existingTableNames.Add(Convert.ToString(tablesEnumerator.Current));
                    }
                }
                catch (Exception)
                {
                    existingTableNames = new List<string>();
                }

                var split = script.Split(new[] { "create table " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var sql in split)
                {
                    var tableName = sql.Substring(0, sql.IndexOf("(", StringComparison.Ordinal));
                    tableName = tableName.Split('.').Last();
                    tableName = tableName.Trim().TrimStart('[').TrimEnd(']');

                    if (existingTableNames.Contains(tableName))
                    {
                        continue;
                    }

                    try
                    {
                        context.Database.ExecuteSqlCommand("CREATE TABLE" + sql);
                    }
                    catch (DbException)
                    {
                        // Ignore when existing
                    }
                }
            }
        }
    }
}