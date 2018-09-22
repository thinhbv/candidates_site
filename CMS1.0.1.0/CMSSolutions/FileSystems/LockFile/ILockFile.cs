using System;

namespace CMSSolutions.FileSystems.LockFile
{
    public interface ILockFile : IDisposable
    {
        void Release();
    }
}