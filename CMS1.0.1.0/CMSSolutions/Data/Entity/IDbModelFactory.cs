using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Autofac;

namespace CMSSolutions.Data.Entity
{
    public interface IDbModelFactory
    {
        DbCompiledModel GetDbCompiledModel<T>(DbConnection connection);
    }

    public class DbModelFactory : IDbModelFactory
    {
        private readonly IComponentContext componentContext;
        private readonly ConcurrentDictionary<string, DbCompiledModel> models;

        public DbModelFactory(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
            models = new ConcurrentDictionary<string, DbCompiledModel>();
        }

        public DbCompiledModel GetDbCompiledModel<T>(DbConnection connection)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            return models.GetOrAdd(typeof(T).Namespace, s =>
            // ReSharper restore AssignNullToNotNullAttribute
            {
                var configurations = componentContext.ResolveNamed<IEnumerable<IEntityTypeConfiguration>>(s);
                var modelBuilder = new DbModelBuilder();
                foreach (dynamic configuration in configurations)
                {
                    modelBuilder.Configurations.Add(configuration);
                }
                var model = modelBuilder.Build(connection);
                return model.Compile();
            });
        }
    }
}