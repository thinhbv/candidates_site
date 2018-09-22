using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using CMSSolutions.Data.Common;

namespace CMSSolutions.Data.Entity
{
    public class DbContextBase : DbContext
    {
        protected DbContextBase(string connectionString)
            : base(connectionString)
        {

        }

        protected DbContextBase(string connectionString, DbCompiledModel model)
            : base(connectionString, model)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        protected DbContextBase(DbConnection connection, DbCompiledModel model)
            : base(connection, model, false)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public IDataProviderFactory DataProviderFactory { get; set; }

        public DbDataReader ExecuteReader(string commandText)
        {
            var connection = Database.Connection;

            //Don't close the connection after command execution
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = commandText;
                var reader = command.ExecuteReader();
                return reader;
            }
        }

        public virtual List<TEntity> ExecuteStoredProcedure<TEntity>(string procedureName)
        {
            var list = new List<TEntity>();
            var connection = Database.Connection;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            try
            {
                var context = ((IObjectContextAdapter)(this)).ObjectContext;
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procedureName;
                    var reader = cmd.ExecuteReader();
                    list = context.Translate<TEntity>(reader).ToList();

                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }

            return list;
        }

        public virtual int ExecuteNonQuery(string procedureName, SqlParameter[] parameter)
        {
            var connection = Database.Connection;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            int result = 0;
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = procedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameter);
                    result = cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }

            return result;
        }

        public virtual int ExecuteNonQuery(string commandText)
        {
            var connection = Database.Connection;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            int result = 0;
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = commandText;
                    result = cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }

            return result;
        }

        public virtual DataSet ExecuteStoredProcedure(string procedureName, params SqlParameter[] parameter)
        {
            var ds = new DataSet();

            var connection = Database.Connection;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procedureName;
                    var dr = cmd.ExecuteReader();
                    ds = (DataSet)dr[0];
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }

            return ds;
        }

        public virtual TEntity ExecuteObject<TEntity>(string procedureName, SqlParameter[] parameter)
        {
            var connection = Database.Connection;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            var obj = Activator.CreateInstance<TEntity>();
            try
            {
                var context = ((IObjectContextAdapter)(this)).ObjectContext;
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procedureName;
                    cmd.Parameters.AddRange(parameter);
                    var reader = cmd.ExecuteReader();
                    obj = context.Translate<TEntity>(reader).FirstOrDefault();

                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }

            return obj;
        }

        public virtual IList<TEntity> ExecuteStoredProcedure<TEntity>(string procedureName, SqlParameter[] parameter)
        {
            var list = new List<TEntity>();
            var connection = Database.Connection;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            try
            {
                var context = ((IObjectContextAdapter)(this)).ObjectContext;
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procedureName;
                    cmd.Parameters.AddRange(parameter);
                    var reader = cmd.ExecuteReader();
                    list = context.Translate<TEntity>(reader).ToList();

                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }

            return list;
        }

        public virtual IList<TEntity> ExecuteStoredProcedure<TEntity>(string commandText, params object[] parameters)
        {
            bool hasOutputParameters = false;
            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    if (parameter == null)
                        continue;

                    if (parameter.Direction == ParameterDirection.InputOutput ||
                        parameter.Direction == ParameterDirection.Output)
                        hasOutputParameters = true;
                }
            }

            if (!hasOutputParameters)
            {
                //no output parameters
                var result = Database.SqlQuery<TEntity>(commandText, parameters).ToList();

                return result;
            }

            var context = ((IObjectContextAdapter)(this)).ObjectContext;

            //Don't close the connection after command execution
            var connection = Database.Connection;

            //open the connection for use
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            //create a command object
            using (var cmd = connection.CreateCommand())
            {
                //command to execute
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.StoredProcedure;

                // move parameters to command object
                foreach (var p in parameters)
                    cmd.Parameters.Add(p);

                //database call
                var reader = cmd.ExecuteReader();

                var result = context.Translate<TEntity>(reader).ToList();

                //close up the reader, we're done saving results
                reader.Close();
                return result;
            }
        }

        protected virtual TEntity Attach<TEntity, TKey>(TEntity entity) where TEntity : BaseEntity<TKey>
        {
            //little hack here until Entity Framework really supports stored procedures
            //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
            var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id.Equals(entity.Id));
            if (alreadyAttached == null)
            {
                //attach new entity
                Set<TEntity>().Attach(entity);
                return entity;
            }

            //entity is already loaded.
            return alreadyAttached;
        }

        public DbParameter CreateParameter(string parameterName, object value)
        {
            return Database.Connection.CreateParameter(parameterName, value);
        }

        public DataSet ExecuteStoredProcedure(string storedProcedure, IEnumerable<DbParameter> parameters)
        {
            return Database.Connection.ExecuteStoredProcedure(storedProcedure, parameters);
        }

        public DataSet ExecuteStoredProcedure(string storedProcedure, IEnumerable<DbParameter> parameters, out Dictionary<string, object> outputValues)
        {
            return Database.Connection.ExecuteStoredProcedure(storedProcedure, parameters, out outputValues);
        }

        public void Insert<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Added;
            SaveChanges();
        }
    }

    public class DefaultDbContext<T> : DbContextBase
    {
        public DefaultDbContext(string connectionString)
            : base(connectionString, null)
        {
        }

        public DefaultDbContext(DbConnection connection, IDbModelFactory dbModelFactory)
            : base(connection, dbModelFactory.GetDbCompiledModel<T>(connection))
        {
            Database.SetInitializer(new CreateTablesIfNotExist<DefaultDbContext<T>>());
        }
    }
}