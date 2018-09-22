using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Transactions;
using CMSSolutions.Configuration;
using CMSSolutions.Security.Cryptography;

namespace CMSSolutions.Data.Entity
{
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContextBase
    {
        private void CreateDatabase(TContext context)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[CMSConfigurationSection.Instance.Data.SettingConnectionString].ConnectionString;
            if (KeyConfiguration.IsEncrypt)
            {
                connectionString = EncryptionExtensions.Decrypt(CMSConfigurationSection.Instance.Data.SettingConnectionString, connectionString);
            }

            var serverName = connectionString.Split(';')[0].Split('=')[1]; 
            var databaseName = connectionString.Split(';')[1].Split('=')[1];
            var userName = connectionString.Split(';')[2].Split('=')[1];
            var password = connectionString.Split(';')[3].Split('=')[1]; 
            var conn = new SqlConnection
            {
                ConnectionString = string.Format("Data Source={0};Initial Catalog=master;User ID={1};Password={2};MultipleActiveResultSets=True;", serverName, userName, password)
            };
            
            try
            {
                var sqlCreateDBQuery = " CREATE DATABASE " + databaseName;
                var cmd = new SqlCommand(sqlCreateDBQuery, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                Thread.Sleep(4000);
                conn.Close(); 
                context.DataProviderFactory.EnsureTables(context);
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                throw new Exception(ex.Message);
            }
        }

        public void InitializeDatabase(TContext context)
        {
            if (CMSConfigurationSection.Instance == null)
            {
                return;
            }

            if (!CMSConfigurationSection.Instance.Data.AutoCreateTables)
            {
                return;
            }

            if (context.DataProviderFactory == null)
            {
                return;
            }

            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }
           
            if (dbExists)
            {
                context.DataProviderFactory.EnsureTables(context);
            }
            else
            {
                CreateDatabase(context);
            }
        }
    }
}