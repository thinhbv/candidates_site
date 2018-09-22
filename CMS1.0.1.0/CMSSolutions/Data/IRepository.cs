using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using CMSSolutions.Data.Entity;

namespace CMSSolutions.Data
{
    public interface IRepository<T, in TKey> where T : BaseEntity<TKey>
    {
        string ConnectionString { get; }

        DbContextBase Context { get; }

        IQueryable<T> Table { get; }

        IQueryable<TTable> GetTable<TTable>() where TTable : class;

        int Count();

        void Delete(T entity, bool deleteRelated = false);

        void Delete<TNavigation>(T entity, Expression<Func<T, IEnumerable<TNavigation>>> expression) where TNavigation : class;

        void DeleteMany(IEnumerable<T> entities);

        T GetById(TKey id);

        void Insert(T entity);

        void InsertMany(IEnumerable<T> entities);

        void Update(T entity);

        void UpdateMany(IEnumerable<T> entities);

        void UpdateMany(Expression<Func<T, bool>> filterExpression, Expression<Func<T, T>> updateExpression);

        IList<T> ExecuteStoredProcedure(string commandText, params object[] parameters);

        DataSet ExecuteStoredProcedure(string procedureName, params SqlParameter[] parameter);

        IList<T> ExecuteStoredProcedure<T>(string procedureName, SqlParameter[] parameter);

        IList<T> ExecuteStoredProcedure<T>(string procedureName);

        int ExecuteNonQuery(string procedureName, SqlParameter[] parameter);

        int ExecuteNonQuery(string commandText);

        TEntity ExecuteObject<TEntity>(string procedureName, SqlParameter[] parameter);

        DataSet ExecuteStoredProcedure(string storedProcedure, IEnumerable<DbParameter> parameters);

        DataSet ExecuteStoredProcedure(string storedProcedure, IEnumerable<DbParameter> parameters, out Dictionary<string, object> outputValues);

        T Attach(T entity);
    }
}