using System;
using System.Threading;

namespace BinaryStorage.Semaphore
{
    public sealed class SemaphoreLock : IDisposable
    {
        // make a singleton to avoid multiple locks
        private SemaphoreLock() { }

        //Singleton Pattern
        private static class SingletonContainer
        {
          public static readonly SemaphoreLock Instance = new SemaphoreLock();
        }

        public static SemaphoreLock Instance => SingletonContainer.Instance;

      //End Singleton Pattern

        // one write thread allowed
        private readonly System.Threading.Semaphore _writeLock = new System.Threading.Semaphore(1, 1);

        // ten read threads allowed
        private readonly System.Threading.Semaphore _readLock = new System.Threading.Semaphore(10, 10);

        public void ApplyWriteLock()
        {
            try
            {
                _writeLock.WaitOne();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(ThreadInterruptedException))
                    throw;
                throw new ThreadInterruptedException();
            }
        }
        public int ReleaseWriteLock()
        {
            return _writeLock.Release();
        }

        public void GetReadLock()
        {
            try
            {
                _readLock.WaitOne();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(ThreadInterruptedException))
                    throw;
                throw new ThreadInterruptedException();
            }
        }
        public int ReleaseReadLock()
        {
            return _readLock.Release();
        }

        public void Dispose()
        {
            _writeLock?.Dispose();
            _readLock?.Dispose();
        }
    }

}