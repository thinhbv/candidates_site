namespace CMSSolutions.IO
{
    public interface IStorageFolder
    {
        string Path { get; }

        string Name { get; }

        IStorageFolder Parent { get; }
    }
}