using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using CMSSolutions.Linq;

namespace CMSSolutions.Data.Entity
{
    public class EntityRepository<T, TKey> : IRepository<T, TKey> where T : BaseEntity<TKey>
    {
        private readonly DbContextBase context;
        private IDbSet<T> entities;

        public EntityRepository(IDbContextFactory contextFactory)
        {
            context = contextFactory.GetContext<T>();
        }

        public EntityRepository(DbContextBase context)
        {
            this.context = context;
        }

        public string ConnectionString 
        { 
            get 
            { 
                return Context.Database.Connection.ConnectionString;
            } 
        }

        public DbContextBase Context
        {
            get
            {
                return context;
            }
        }

        public virtual IQueryable<T> Table
        {
            get { return Entities; }
        }

        public IQueryable<TTable> GetTable<TTable>() where TTable : class
        {
            return context.Set<TTable>();
        }

        protected IDbSet<T> Entities
        {
            get { return entities ?? (entities = context.Set<T>()); }
        }

        public int Count()
        {
            return Entities.Count();
        }

        public void Delete(T entity, bool deleteRelated = false)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                if (deleteRelated)
                {
                    var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                    var type = typeof(T);
                    var properties = type.GetProperties();
                    var edmType = GetEdmType(objectContext.MetadataWorkspace, typeof(T));

                    foreach (NavigationProperty member in edmType.Members.Where(x => x.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty))
                    {
                        if (member.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One
                            && member.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                        {
                            var property = properties.First(x => x.Name == member.Name);
                            var items = property.GetValue(entity) as IEnumerable<object>;

                            if (items == null)
                            {
                                context.Entry(entity).Collection(member.Name).Load();
                                items = property.GetValue(entity) as IEnumerable<object>;
                            }

                            if (items != null)
                            {
                                while (items.Any())
                                {
                                    context.Entry(items.ElementAt(0)).State = EntityState.Deleted;
                                }    
                            }
                        }
                    }
                }

                context.Entry(entity).State = EntityState.Deleted;
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(Constants.Messages.CannotDeleteRecord, dbEx);
            }
        }

        public void Delete<TNavigation>(T entity, Expression<Func<T, IEnumerable<TNavigation>>> expression) where TNavigation : class
        {
            var items = expression.Compile()(entity);
            if (items == null)
            {
                return;
            }

            while (items.Any())
            {
                context.Entry(items.ElementAt(0)).State = EntityState.Deleted;
            }
        }

        public void DeleteMany(IEnumerable<T> items)
        {
            if (!items.Any())
            {
                return;
            }

            try
            {
                if (items.Count() < 100)
                {
                    var collection = new List<T>(items);
                    foreach (var item in collection)
                    {
                        context.Entry(item).State = EntityState.Deleted;
                    }

                    context.SaveChanges();
                    return;
                }

                var ids = items.Select(x => x.Id).ToList();
                Table.Delete(x => ids.Contains(x.Id));
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = dbEx.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors)
                    .Aggregate(string.Empty, (current, validationError) => current + (System.Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage)));

                throw new Exception(msg, dbEx);
            }
        }

        public virtual T GetById(TKey id)
        {
            return Entities.Find(id);
        }

        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Entities.Add(entity);
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = dbEx.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors)
                    .Aggregate(string.Empty, (current, validationError) => current + (string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + System.Environment.NewLine));

                throw new Exception(msg, dbEx);
            }
        }

        public void InsertMany(IEnumerable<T> items)
        {
            if (!items.Any())
            {
                return;
            }

            var sqlConnection = context.Database.Connection as SqlConnection;
            if (sqlConnection != null)
            {
                using (var bulkInsert = new SqlBulkCopy(sqlConnection))
                {
                    bulkInsert.BatchSize = 1000;
                    bulkInsert.DestinationTableName = GetTableName();

                    var table = new DataTable();
                    var props = TypeDescriptor.GetProperties(typeof(T))

                        //Dirty hack to make sure we only have system data types
                        //i.e. filter out the relationships/collections
                                               .Cast<PropertyDescriptor>()

                        // ReSharper disable PossibleNullReferenceException
                                               .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))

                        // ReSharper restore PossibleNullReferenceException
                                               .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        bulkInsert.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];
                    foreach (var item in items)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }

                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        sqlConnection.Open();
                    }

                    bulkInsert.WriteToServer(table);
                }
            }
            else
            {
                try
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.ValidateOnSaveEnabled = false;

                    var count = 0;
                    foreach (var item in items)
                    {
                        Entities.Add(item);
                        count++;

                        if (count == 100)
                        {
                            context.SaveChanges();
                            count = 0;
                        }
                    }

                    if (count > 0)
                    {
                        context.SaveChanges();
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    var msg = dbEx.EntityValidationErrors
                        .SelectMany(validationErrors => validationErrors.ValidationErrors)
                        .Aggregate(string.Empty, (current, validationError) => current + (string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + System.Environment.NewLine));

                    throw new Exception(msg, dbEx);
                }
            }
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                var isAttached = false;

                if (context.Entry(entity).State == EntityState.Detached)
                {
                    // Try attach into context
                    var hashCode = entity.GetHashCode();
                    foreach (var obj in Entities.Local)
                    {
                        if (obj.GetHashCode() == hashCode)
                        {
                            context.Entry(obj).CurrentValues.SetValues(entity);
                            isAttached = true;
                            break;
                        }
                    }

                    if (!isAttached)
                    {
                        entity = Entities.Attach(entity);
                        context.Entry(entity).State = EntityState.Modified;
                    }
                }
                else
                {
                    // Set the entity's state to modified
                    context.Entry(entity).State = EntityState.Modified;
                }

                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = dbEx.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors)
                    .Aggregate(string.Empty, (current, validationError) => current + (System.Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage)));

                throw new Exception(msg, dbEx);
            }
        }

        public virtual void UpdateMany(IEnumerable<T> items)
        {
            try
            {
                if (items == null)
                    throw new ArgumentNullException("items");

                foreach (var entity in items)
                {
                    if (context.Entry(entity).State == EntityState.Detached)
                    {
                        Entities.Attach(entity);
                    }

                    // Set the entity's state to modified
                    context.Entry(entity).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = dbEx.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors)
                    .Aggregate(string.Empty, (current, validationError) => current + (System.Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage)));

                throw new Exception(msg, dbEx);
            }
        }

        public virtual void UpdateMany(Expression<Func<T, bool>> filterExpression, Expression<Func<T, T>> updateExpression)
        {
            Entities.Update(filterExpression, updateExpression);
        }

        public IList<T> ExecuteStoredProcedure(string commandText, params object[] parameters)
        {
            return context.ExecuteStoredProcedure<T>(commandText, parameters);
        }

        public DataSet ExecuteStoredProcedure(string procedureName, params SqlParameter[] parameter)
        {
            return context.ExecuteStoredProcedure(procedureName, parameter);
        }

        public IList<T> ExecuteStoredProcedure<T>(string procedureName, SqlParameter[] parameter)
        {
            return context.ExecuteStoredProcedure<T>(procedureName, parameter);
        }

        public IList<T> ExecuteStoredProcedure<T>(string procedureName)
        {
            return context.ExecuteStoredProcedure<T>(procedureName);
        }

        public int ExecuteNonQuery(string procedureName, SqlParameter[] parameter)
        {
            return context.ExecuteNonQuery(procedureName, parameter);
        }

        public int ExecuteNonQuery(string commandText)
        {
            return context.ExecuteNonQuery(commandText);
        }

        public TEntity ExecuteObject<TEntity>(string procedureName, SqlParameter[] parameter)
        {
            return context.ExecuteObject<TEntity>(procedureName, parameter);
        }

        public DataSet ExecuteStoredProcedure(string storedProcedure, IEnumerable<DbParameter> parameters)
        {
            return context.ExecuteStoredProcedure(storedProcedure, parameters);
        }

        public DataSet ExecuteStoredProcedure(string storedProcedure, IEnumerable<DbParameter> parameters, out Dictionary<string, object> outputValues)
        {
            return context.ExecuteStoredProcedure(storedProcedure, parameters, out outputValues);
        }

        public T Attach(T entity)
        {
            foreach (var localEntity in Entities.Local)
            {
                if (entity.GetHashCode() == localEntity.GetHashCode())
                {
                    return localEntity;
                }
            }

            return Entities.Attach(entity);
        }

        private static StructuralType GetEdmType(MetadataWorkspace workspace, Type clrType)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException("workspace");
            }

            if (clrType == null)
            {
                throw new ArgumentNullException("clrType");
            }

            if (clrType.IsPrimitive || clrType == typeof(object))
            {
                // want to avoid loading searching system assemblies for
                // types we know aren't entity or complex types
                return null;
            }

            // We first locate the EdmType in "OSpace", which matches the name and namespace of the CLR type
            EdmType edmType;
            do
            {
                if (!workspace.TryGetType(clrType.Name, clrType.Namespace, DataSpace.OSpace, out edmType))
                {
                    // If EF could not find this type, it could be because it is not loaded into
                    // its current workspace.  In this case, we explicitly load the assembly containing
                    // the CLR type and try again.
                    workspace.LoadFromAssembly(clrType.Assembly);
                    workspace.TryGetType(clrType.Name, clrType.Namespace, DataSpace.OSpace, out edmType);
                }
            }
            while (edmType == null && (clrType = clrType.BaseType) != typeof(object) && clrType != null);

            // Next we locate the StructuralType from the EdmType.
            // This 2-step process is necessary when the types CLR namespace does not match Edm namespace.
            // Look at the EdmEntityTypeAttribute on the generated entity classes to see this Edm namespace.
            StructuralType structuralType = null;
            if (edmType != null &&
                (edmType.BuiltInTypeKind == BuiltInTypeKind.EntityType || edmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType))
            {
                workspace.TryGetEdmSpaceType((StructuralType)edmType, out structuralType);
            }

            return structuralType;
        }

        private string GetTableName()
        {
            var set = context.Set<T>();
            var regex = new Regex("FROM (?<table>.*) AS");
            var sql = set.ToString();
            var match = regex.Match(sql);

            return match.Groups["table"].Value;
        }
    }
}