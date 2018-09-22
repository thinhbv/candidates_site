using Autofac;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Data
{
    [Feature(Constants.Areas.Core)]
    public class DataModule : Module
    {
        public int Priority { get { return int.MaxValue; } }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(EntityRepository<,>)).As(typeof(IRepository<,>)).InstancePerDependency();
            builder.RegisterType<DbModelFactory>().As<IDbModelFactory>().SingleInstance();
            builder.RegisterType<DbContextFactory>().As<IDbContextFactory>().InstancePerDependency();
        }
    }
}