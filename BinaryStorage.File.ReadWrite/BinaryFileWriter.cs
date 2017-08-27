using System;
using System.IO;
using System.Text;
using BinaryStorage.Semaphore;

namespace BinaryStorage.File.ReadWrite
{
    public sealed class BinaryFileWriter : IDisposable
    {
        private System.IO.BinaryWriter _bDataFile;
        private readonly object _lockObject = new object();

        public BinaryFileWriter(string filePath)
        {
            _bDataFile = new BinaryWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write,
                FileShare.ReadWrite));
        }

        public void Write(string text)
        {
            lock (_lockObject)
            {
                var len = text.Length;
                _bDataFile.Write(len);
                _bDataFile.Write(BitConverter.GetBytes(len));
                _bDataFile.Write( Encoding.UTF8.GetBytes(text));    
            }            
        }

        private void Close()
        {
            if (_bDataFile == null) return;
            lock (_lockObject)
            {
                if (_bDataFile == null) return;
                _bDataFile.Close();
            }
        }
        public void Dispose()
        {
            Close();
            if (_bDataFile == null) return;
            lock (_lockObject)
            {
                if (_bDataFile == null) return;
                _bDataFile.Dispose();
                _bDataFile = null;
            }
        }
    }
}