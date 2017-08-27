using System;
using System.IO;
using System.Text;

namespace BinaryStorage.File.ReadWrite
{
    public sealed class BinaryFileReader : IDisposable
    {
        private System.IO.BinaryReader _bDataFile;
        private readonly object _lockObject = new object();

        public BinaryFileReader(string filePath)
        {

            _bDataFile = new System.IO.BinaryReader(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read,
                FileShare.ReadWrite));
        }

        public string Read()
        {
            lock (_lockObject)
            {                               
                var len = BitConverter.ToInt32( _bDataFile.ReadBytes(4), 0);
                _bDataFile.ReadBytes(4); //advance past beginning of line
                var retVal = Encoding.UTF8.GetString(_bDataFile.ReadBytes(len));
                return retVal;
            }            
        }

        private void Close()
        {
            if (_bDataFile == null) return;
            lock (_lockObject)
            {
                if (_bDataFile == null) return;
                _bDataFile.Close();
                _bDataFile = null;
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
             }
        }

    }
}