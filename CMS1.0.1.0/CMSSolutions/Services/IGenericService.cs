using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Services;
using CMSSolutions.Data;
using CMSSolutions.Events;
using CMSSolutions.Extensions;
using CMSSolutions.Linq;
using CMSSolutions.Linq.Dynamic;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Services
{
    [WebService(Namespace = Constants.NamespaceSite)]
    [WebServiceBinding(ConformsTo = Constants.EnumWsiProfiles)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public interface IGenericService<TRecord, in TKey> where TRecord : BaseEntity<TKey>
    {
        int Count(Expression<Func<TRecord, bool>> predicate = null);

        SqlParameter AddInputParameter<T>(string name, T value);

        SqlParameter AddOutputParameter(string name, int size, SqlDbType type);

        SqlParameter AddInputParameter(string name, SqlDbType type, int size, string columnName);

        void Delete(TRecord entity);

        void Delete(TRecord entity, bool deleteRelated);

        void Delete<TNavigation>(TRecord entity, Expression<Func<TRecord, IEnumerable<TNavigation>>> expression) where TNavigation : class;

        void DeleteMany(IEnumerable<TRecord> records);

        TRecord GetById(TKey id);

        TRecord GetRecord(Expression<Func<TRecord, bool>> predicate, params Expression<Func<TRecord, dynamic>>[] includePaths);

        IList<TRecord> GetRecords();

        IList<TRecord> GetRecords(Expression<Func<TRecord, bool>> predicate, params Expression<Func<TRecord, dynamic>>[] includePaths);

        IList<TRecord> GetRecords(ControlGridFormRequest request, out int totalRecords, Expression<Func<TRecord, bool>> predicate = null, params Expression<Func<TRecord, dynamic>>[] includePaths);

        IList<TRecord> GetRecords(int pageIndex, int pageSize, out int totalRecords, Expression<Func<TRecord, bool>> predicate = null, params Expression<Func<TRecord, dynamic>>[] includePaths);

        IList<TResult> GetFromRecords<TResult>(Expression<Func<TRecord, bool>> predicate, Func<TRecord, TResult> selector);

        void Insert(TRecord record);

        void InsertMany(IEnumerable<TRecord> records);

        void Save(TRecord record);

        void Update(TRecord record);

        void UpdateMany(IEnumerable<TRecord> records);

        void UpdateMany(Expression<Func<TRecord, bool>> filterExpression, Expression<Func<TRecord, TRecord>> updateExpression);

        TRecord Attach(TRecord record);

        List<T> ExecuteSqlCommand<T>(string commandText);

        List<T> ExecuteReader<T>(string storedProcedureName);

        List<T> ExecuteReader<T>(string storedProcedureName, params SqlParameter[] parameters);

        List<T> ExecuteReader<T>(string storedProcedureName, string columnNameOut, out int totalRecords, params SqlParameter[] parameters);
        
        int ExecuteNonQuery(string storedProcedureName, string columnNameOut, out string errorMessages,params SqlParameter[] parameters);
        
        List<T> ExecuteReader<T>(string storedProcedureName, string columnNameOut, out int totalRecords);
        
        DataSet ExecuteReader(string storedProcedureName, params SqlParameter[] parameters);
        
        DataSet ExecuteReader(string storedProcedureName);
        
        T ExecuteReaderRecord<T>(string storedProcedureName, params SqlParameter[] parameters);
        
        object ExecuteReaderResult(string storedProcedureName, params SqlParameter[] parameters);
        
        int ExecuteNonQuery(string storedProcedureName, DataTable listDetails, params SqlParameter[] parameters);
        
        int ExecuteNonQuery(string storedProcedureName, params SqlParameter[] parameters);

        string SqlConnectionString { get; set; }
    }

    public abstract class GenericService<TRecord, TKey> : WebService, IGenericService<TRecord, TKey> where TRecord : BaseEntity<TKey>
    {
        protected readonly IRepository<TRecord, TKey> Repository;
        private readonly IEventBus eventBus;
        public string SqlConnectionString { get; set; }

        protected GenericService(IRepository<TRecord, TKey> repository, IEventBus eventBus)
        {
            Repository = repository;
            this.eventBus = eventBus;
            SqlConnectionString = Repository.ConnectionString;
        }

        protected virtual string BuildCacheKey(string key)
        {
            return key;
        }

        protected virtual IOrderedQueryable<TRecord> MakeDefaultOrderBy(IQueryable<TRecord> queryable)
        {
            return queryable.OrderBy(x => x.Id);
        }

        #region IGenericService<TRecord> Members

        public virtual SqlParameter AddInputParameter<T>(
            string name,
            T value)
        {
            var parameter = new SqlParameter
            {
                ParameterName = name,
                Value = value,
                Direction = ParameterDirection.Input
            };

            return parameter;
        }

        public virtual SqlParameter AddOutputParameter(
            string name,
            int size,
            SqlDbType type)
        {
            var parameter = new SqlParameter
            {
                ParameterName = name,
                SqlDbType = type,
                Size = size,
                Direction = ParameterDirection.Output
            };

            return parameter;
        }

        public virtual SqlParameter AddInputParameter(
            string name,
            SqlDbType type,
            int size,
            string columnName)
        {
            var parameter = new SqlParameter
            {
                ParameterName = name,
                SqlDbType = type,
                Size = size,
                SourceColumn = columnName,
                Direction = ParameterDirection.Input
            };

            return parameter;
        }

        public int Count(Expression<Func<TRecord, bool>> predicate = null)
        {
            return predicate == null ? Repository.Table.Count() : Repository.Table.Count(predicate);
        }

        public virtual void Delete(TRecord record)
        {
            eventBus.NotifyContentRemoving(record);
            Repository.Delete(record);
            eventBus.NotifyContentRemoved(record);
        }

        public virtual void Delete(TRecord record, bool deleteRelated)
        {
            Repository.Delete(record, deleteRelated);
        }

        public void Delete<TNavigation>(TRecord entity, Expression<Func<TRecord, IEnumerable<TNavigation>>> expression) where TNavigation : class
        {
            Repository.Delete(entity, expression);
        }

        public virtual void DeleteMany(IEnumerable<TRecord> records)
        {
            Repository.DeleteMany(records);
        }

        public virtual TRecord GetById(TKey id)
        {
            return Repository.GetById(id);
        }

        public TRecord GetRecord(Expression<Func<TRecord, bool>> predicate, params Expression<Func<TRecord, dynamic>>[] includePaths)
        {
            var query = Repository.Table;

            if (includePaths != null && includePaths.Length > 0)
            {
                query = includePaths.Aggregate(query, (current, path) => current.Include(path));
            }

            return predicate == null
                ? MakeDefaultOrderBy(query).FirstOrDefault()
                : query.AsExpandable().Where(predicate).FirstOrDefault();
        }

        public virtual IList<TRecord> GetRecords()
        {
            var query = Repository.Table;

            return MakeDefaultOrderBy(query).ToList();
        }

        public virtual IList<TRecord> GetRecords(Expression<Func<TRecord, bool>> predicate, params Expression<Func<TRecord, dynamic>>[] includePaths)
        {
            var query = Repository.Table;

            if (includePaths != null && includePaths.Length > 0)
            {
                query = includePaths.Aggregate(query, (current, path) => current.Include(path));
            }

            return predicate == null
                ? MakeDefaultOrderBy(query).ToList()
                : MakeDefaultOrderBy(query).AsExpandable().Where(predicate).ToList();
        }

        public IList<TRecord> GetRecords(ControlGridFormRequest request, out int totalRecords, Expression<Func<TRecord, bool>> predicate = null, params Expression<Func<TRecord, dynamic>>[] includePaths)
        {
            var queryable = Repository.Table;

            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                queryable = request.SortDirection ?
                    queryable.OrderBy(request.SortColumn) :
                    queryable.OrderBy(request.SortColumn + " DESC");
            }
            else
            {
                queryable = MakeDefaultOrderBy(queryable);
            }

            if (includePaths != null && includePaths.Length > 0)
            {
                queryable = includePaths.Aggregate(queryable, (current, path) => current.Include(path));
            }

            // Filtering
            if (request.Filters != null && request.Filters.Count > 0)
            {
                var expression = GetFilters(request.Filters);
                queryable = queryable.Where(expression);
            }

            if (predicate != null)
            {
                queryable = queryable.AsExpandable().Where(predicate);
            }

            totalRecords = queryable.Count();

            var items = queryable.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();

            return items;
        }

        public IList<TResult> GetFromRecords<TResult>(Expression<Func<TRecord, bool>> predicate, Func<TRecord, TResult> selector)
        {
            var query = Repository.Table;
            return predicate == null
                ? MakeDefaultOrderBy(query).Select(selector).ToList()
                : MakeDefaultOrderBy(query).AsExpandable().Where(predicate).Select(selector).ToList();
        }

        public virtual IList<TRecord> GetRecords(int pageIndex, int pageSize, out int totalRecords, Expression<Func<TRecord, bool>> predicate = null, params Expression<Func<TRecord, dynamic>>[] includePaths)
        {
            var query = Repository.Table;

            if (includePaths != null && includePaths.Length > 0)
            {
                query = includePaths.Aggregate(query, (current, path) => current.Include(path));
            }

            if (predicate != null)
            {
                query = query.AsExpandable().Where(predicate);
            }

            totalRecords = query.Count();

            return MakeDefaultOrderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public virtual void Insert(TRecord record)
        {
            eventBus.NotifyContentCreating(record);
            record.OnInserting();
            Repository.Insert(record);
            eventBus.NotifyContentCreated(record);
        }

        public virtual void InsertMany(IEnumerable<TRecord> records)
        {
            Repository.InsertMany(records);
        }

        public virtual void Save(TRecord record)
        {
            if (record.IsTransient())
            {
                Insert(record);
            }
            else
            {
                Update(record);
            }
        }

        public virtual void Update(TRecord record)
        {
            eventBus.NotifyContentUpdating(record);
            Repository.Update(record);
            eventBus.NotifyContentUpdated(record);
        }

        public void UpdateMany(IEnumerable<TRecord> records)
        {
            Repository.UpdateMany(records);
        }

        public void UpdateMany(Expression<Func<TRecord, bool>> filterExpression, Expression<Func<TRecord, TRecord>> updateExpression)
        {
            Repository.UpdateMany(filterExpression, updateExpression);
        }

        public TRecord Attach(TRecord record)
        {
            return Repository.Attach(record);
        }

        #endregion IGenericService<TRecord> Members

        #region Helpers

        private static Expression<Func<TRecord, bool>> GetFilters(IEnumerable<ControlGridFilter> filters)
        {
            var expression = PredicateBuilder.True<TRecord>();
            return filters.Aggregate(expression, (current, filter) => current.And(GetFilter(filter)));
        }

        private static Expression<Func<TRecord, bool>> GetFilter(ControlGridFilter filter)
        {
            var parameter = Expression.Parameter(typeof(TRecord), "x");
            var property = Expression.PropertyOrField(parameter, filter.Name);
            
            switch (filter.Operator)
            {
                case ControlGridFilterOperator.Equal:
                {
                    if (property.Type == typeof(string))
                    {
                        var value = filter.Value.ToLower();
                        var valueConstant = Expression.Constant(value);
                        var method = typeof (string).GetMethod("ToLower", new Type[] {});
                        var methodCallExpression = Expression.Call(property, method);
                        return Expression.Lambda<Func<TRecord, bool>>(Expression.Equal(methodCallExpression, valueConstant), parameter);                        
                    }
                    else
                    {
                        var value = Convert.ChangeType(filter.Value, property.Type);
                        var valueConstant = Expression.Constant(value);
                        return Expression.Lambda<Func<TRecord, bool>>(Expression.Equal(property, valueConstant), parameter);                    
                    }
                }
                case ControlGridFilterOperator.NotEqual:
                    break;

                case ControlGridFilterOperator.Less:
                    break;

                case ControlGridFilterOperator.LessOrEqual:
                    break;

                case ControlGridFilterOperator.Greater:
                    break;

                case ControlGridFilterOperator.GreaterOrEqual:
                    break;

                case ControlGridFilterOperator.BeginsWith:
                    break;

                case ControlGridFilterOperator.NotBeginsWith:
                    break;

                case ControlGridFilterOperator.IsIn:
                {
                    var type = typeof (List<>).MakeGenericType(property.Type);
                    var values = CreateListOfType(type, filter.Value.Split(',').Select(x => ConvertToType(x, property.Type)));
                    var containsMethod = type.GetMethod("Contains", new[] { property.Type });
                    var valueConstant = Expression.Constant(values, type);
                    var methodCallExpression = Expression.Call(valueConstant, containsMethod, new Expression[] { property });
                    return Expression.Lambda<Func<TRecord, bool>>(methodCallExpression, parameter);
                }

                case ControlGridFilterOperator.NotIsIn:
                    break;

                case ControlGridFilterOperator.EndsWith:
                    break;

                case ControlGridFilterOperator.NotEndsWith:
                    break;

                case ControlGridFilterOperator.Contains:
                {
                    var value = Expression.Constant(filter.Value);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { });
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var methodCallExpression = Expression.Call(property, toLowerMethod, new Expression[] { });
                    methodCallExpression = Expression.Call(methodCallExpression, containsMethod, new Expression[] { value });
                    return Expression.Lambda<Func<TRecord, bool>>(methodCallExpression, parameter);
                }
                case ControlGridFilterOperator.NotContains:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            throw new NotImplementedException();
        }

        private static object ConvertToType(string value, Type target)
        {
            if (target == typeof(Guid))
            {
                return new Guid(value);
            }
            return Convert.ChangeType(value, target);
        }

        private static object CreateListOfType(Type listType, IEnumerable<object> values)
        {
            var list = Activator.CreateInstance(listType);
            var addMethod = listType.GetMethod("Add");
            foreach (var value in values)
            {
                addMethod.Invoke(list, new[] {value});
            }
            return list;
        }

        #endregion

        #region Call Store

        public virtual List<T> ExecuteSqlCommand<T>(
    string commandText)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                var list = new List<T>();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = commandText;
                    command.CommandType = CommandType.Text;
                    var reader = command.ExecuteReader();

                    var properties = typeof(T).GetProperties();
                    while (reader.Read())
                    {
                        var local = Activator.CreateInstance<T>();
                        foreach (var info in properties)
                        {
                            try
                            {
                                var name = ReflectionExtensions.GetAttributeDisplayName(info);
                                if (name == Constants.NotMapped)
                                {
                                    continue;
                                }

                                var data = reader[name];
                                if (data.GetType() != typeof(DBNull))
                                {
                                    info.SetValue(local, data, null);
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }  
                        }

                        list.Add(local);
                    }

                    reader.Close();
                }

                connection.Close();
                return list;
            }
        }

        public virtual List<T> ExecuteReader<T>(
            string storedProcedureName)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                var list = new List<T>();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    var reader = command.ExecuteReader();

                    var properties = typeof(T).GetProperties();
                    while (reader.Read())
                    {
                        var local = Activator.CreateInstance<T>();
                        foreach (var info in properties)
                        {
                            try
                            {
                                var name = ReflectionExtensions.GetAttributeDisplayName(info);
                                if (name == Constants.NotMapped)
                                {
                                    continue;
                                }

                                var data = reader[name];
                                if (data.GetType() != typeof(DBNull))
                                {
                                    info.SetValue(local, data, null);
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            } 
                        }

                        list.Add(local);
                    }

                    reader.Close();
                }

                connection.Close();
                return list;
            }
        }

        public virtual List<T> ExecuteReader<T>(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                var list = new List<T>();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);
                    var reader = command.ExecuteReader();

                    var properties = typeof(T).GetProperties();
                    while (reader.Read())
                    {
                        var local = Activator.CreateInstance<T>();
                        foreach (var info in properties)
                        {
                            try
                            {
                                var name = ReflectionExtensions.GetAttributeDisplayName(info);
                                if (name == Constants.NotMapped)
                                {
                                    continue;
                                }

                                var data = reader[name];
                                if (data.GetType() != typeof(DBNull))
                                {
                                    info.SetValue(local, data, null);
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }

                        list.Add(local);
                    }

                    reader.Close();
                }

                connection.Close();
                return list;
            }
        }

        public virtual List<T> ExecuteReader<T>(
            string storedProcedureName,
            string columnNameOut,
            out int totalRecords,
            params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            totalRecords = 0;
            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                var list = new List<T>();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);
                    command.Parameters.Add(AddOutputParameter(columnNameOut, 10, SqlDbType.Int));
                    var reader = command.ExecuteReader();
                    var properties = typeof(T).GetProperties();
                    while (reader.Read())
                    {
                        var local = Activator.CreateInstance<T>();
                        foreach (var info in properties)
                        {
                            try
                            {
                                var name = ReflectionExtensions.GetAttributeDisplayName(info);
                                if (name == Constants.NotMapped)
                                {
                                    continue;
                                }

                                var data = reader[name];
                                if (data.GetType() != typeof(DBNull))
                                {
                                    info.SetValue(local, data, null);
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                        list.Add(local);
                    }
                    reader.Close();

                    if (command.Parameters[columnNameOut].Value != null)
                    {
                        totalRecords = (int)command.Parameters[columnNameOut].Value;
                    }
                }           
                connection.Close();

                return list;
            }
        }

        public virtual int ExecuteNonQuery(
            string storedProcedureName,
            string columnNameOut,
            out string errorMessages,
            params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            using (var connection = new SqlConnection(SqlConnectionString))
            {
                var result = 0;
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);
                    command.Parameters.Add(AddOutputParameter(columnNameOut, 250, SqlDbType.NVarChar));
                    result = command.ExecuteNonQuery();
                    if (command.Parameters[columnNameOut].Value != null)
                    {
                        errorMessages = command.Parameters[columnNameOut].Value.ToString();
                    }
                    else
                    {
                        errorMessages = string.Empty;
                    }
                }

                connection.Close();
                return result;
            }
        }

        public virtual List<T> ExecuteReader<T>(
            string storedProcedureName,
            string columnNameOut,
            out int totalRecords)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            totalRecords = 0;
            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                var list = new List<T>();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(AddOutputParameter(columnNameOut, 50, SqlDbType.Int));
                    var reader = command.ExecuteReader();
                    var properties = typeof(T).GetProperties();
                    while (reader.Read())
                    {
                        var local = Activator.CreateInstance<T>();
                        foreach (var info in properties)
                        {
                            try
                            {
                                var name = ReflectionExtensions.GetAttributeDisplayName(info);
                                if (name == Constants.NotMapped)
                                {
                                    continue;
                                }

                                var data = reader[name];
                                if (data.GetType() != typeof(DBNull))
                                {
                                    info.SetValue(local, data, null);
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }

                        list.Add(local);
                    }

                    reader.Close();

                    if (command.Parameters[columnNameOut].Value != null)
                    {
                        totalRecords = (int)command.Parameters[columnNameOut].Value;
                    }
                }

                connection.Close();
                return list;
            }
        }

        public virtual DataSet ExecuteReader(
           string storedProcedureName,
           params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            var ds = new DataSet();
            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = storedProcedureName;
                    command.Connection = connection;
                    command.Parameters.AddRange(parameters);
                    var da = new SqlDataAdapter(command);
                    da.Fill(ds);
                }

                connection.Close();
            }

            return ds;
        }

        public virtual DataSet ExecuteReader(
           string storedProcedureName,
            string columnNameOut,
            out int totalRecords,
            params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }
            totalRecords = 0;
            var ds = new DataSet();
            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = storedProcedureName;
                    command.Connection = connection;
                    command.Parameters.AddRange(parameters);
                    command.Parameters.Add(AddOutputParameter(columnNameOut, 10, SqlDbType.Int));
                    var da = new SqlDataAdapter(command);
                    da.Fill(ds);
                    if (command.Parameters[columnNameOut].Value != null)
                    {
                        totalRecords = (int)command.Parameters[columnNameOut].Value;
                    }
                }

                connection.Close();
            }

            return ds;
        }

        public virtual DataSet ExecuteReader(
          string storedProcedureName)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            var ds = new DataSet();
            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = storedProcedureName;
                    command.Connection = connection;
                    var da = new SqlDataAdapter(command);
                    da.Fill(ds);
                }

                connection.Close();
            }

            return ds;
        }

        public virtual T ExecuteReaderRecord<T>(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                T local = default(T);
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);
                    var reader = command.ExecuteReader();

                    var properties = typeof(T).GetProperties();
                    while (reader.Read())
                    {
                        local = Activator.CreateInstance<T>();
                        foreach (var info in properties)
                        {
                            try
                            {
                                var name = ReflectionExtensions.GetAttributeDisplayName(info);
                                if (name == Constants.NotMapped)
                                {
                                    continue;
                                }

                                var data = reader[name];
                                if (data.GetType() != typeof(DBNull))
                                {
                                    info.SetValue(local, data, null);
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            } 
                        }
                        break;
                    }

                    reader.Close();
                }

                connection.Close();
                return local;
            }
        }

        public virtual object ExecuteReaderResult(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                var local = new object();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        local = reader[0];
                        break;
                    }

                    reader.Close();
                }

                connection.Close();

                return local;
            }
        }

        public int ExecuteNonQuery(
           string storedProcedureName,
           DataTable listDetails,
           params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            var data = 0;
            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                var adapter = new SqlDataAdapter();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);

                    adapter.InsertCommand = command;
                    data = adapter.Update(listDetails);
                }

                connection.Close();
            }

            return data;
        }

        public int ExecuteNonQuery(
           string storedProcedureName,
           params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            var data = 0;
            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);
                    data = command.ExecuteNonQuery();
                }

                connection.Close();
            }

            return data;
        }

        #endregion
    }
}