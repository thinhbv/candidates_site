using Autofac;
namespace CMSSolutions.Environment
{
    public interface IStartupTask : IDependency
    {
        void Run();
    }
}
