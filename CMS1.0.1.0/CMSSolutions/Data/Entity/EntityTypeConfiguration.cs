namespace CMSSolutions.Data.Entity
{
    public interface IEntityTypeConfiguration : IDependency
    {
    }

    public abstract class EntityTypeConfiguration<T> : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<T>, IEntityTypeConfiguration where T : class
    {
    }
}
