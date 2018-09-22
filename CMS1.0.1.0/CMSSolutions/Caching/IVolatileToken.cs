namespace CMSSolutions.Caching
{
    public interface IVolatileToken
    {
        bool IsCurrent { get; }
    }
}