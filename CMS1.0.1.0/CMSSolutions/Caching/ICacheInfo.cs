namespace CMSSolutions.Caching
{
    public interface ICacheInfo
    {
        void Add(string key, object item);

        void Remove(string key);

        void RemoveAll();

        object Get(string key);
    }
}
