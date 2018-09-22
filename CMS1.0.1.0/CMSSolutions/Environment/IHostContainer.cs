namespace CMSSolutions.Environment
{
    public interface IHostContainer
    {
        T Resolve<T>();
    }
}