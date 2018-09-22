namespace CMSSolutions.Caching
{
    public interface ICacheContextAccessor
    {
        IAcquireContext Current { get; set; }
    }
}