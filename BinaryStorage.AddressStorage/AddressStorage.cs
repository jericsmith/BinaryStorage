using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using BinaryStorage.Abstractions;
using BinaryStorage.Semaphore;

namespace BinaryStorage.AddressStorage
{
    public sealed class AddressStorage : IAddressStorage
    {
        private const string Addresslineending = "--";
        private const string InvalidAddress = "Address is invalid";

        private readonly string _dataFilePath;
        private File.ReadWrite.BinaryFileReader _reader;
        private File.ReadWrite.BinaryFileWriter _writer;
        private static readonly object LockObject = new object();

        private AddressStorage(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
        }

        public static AddressStorage StorageInstance => SingletonContainer.StorageContainerInstance;

        public void WriteAddress(IAddress address)
        {
            SemaphoreLock.Instance.ApplyWriteLock();
            _reader?.Dispose();
            if (_writer == null)
                _writer = new File.ReadWrite.BinaryFileWriter(_dataFilePath);
            if (!address.IsValid())
            {
                SemaphoreLock.Instance.ReleaseWriteLock();
                throw new InvalidDataException(InvalidAddress);
            }
            _writer.Write(address.Line1);
            if (!string.IsNullOrEmpty(address.Line2))
                _writer.Write(address.Line2);
            _writer.Write(Addresslineending);
            _writer.Write(address.City);
            _writer.Write(address.State);
            _writer.Write(address.PostalCode);
            SemaphoreLock.Instance.ReleaseWriteLock();
        }

        public IEnumerable<IAddress> GetAddresses()
        {
            SemaphoreLock.Instance.GetReadLock();
            _writer?.Dispose();
            if (_reader == null)
                _reader = new File.ReadWrite.BinaryFileReader(_dataFilePath);

            var addresses = new List<IAddress>();
            while (true)
                try
                {
                    var address = new Address();
                    while (true)
                    {
                        address.Line1 = _reader.Read();
                        var recordLine = _reader.Read();
                        if (recordLine != Addresslineending)
                            address.Line2 = recordLine;
                        address.City = _reader.Read();
                        address.State = _reader.Read();
                        address.PostalCode = _reader.Read();
                        addresses.Add(address);
                        break;
                    }
                }
                catch (Exception)
                {
                    break;
                }
            SemaphoreLock.Instance.ReleaseReadLock();
            return addresses;
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _writer?.Dispose();
        }

        private static class SingletonContainer
        {
            private const string DataStoragePathKey = "dataStoragePath";
            private static readonly string DataStoragePath = ConfigurationManager.AppSettings[DataStoragePathKey];

            private static readonly AddressStorage ContainerInstance =
                new AddressStorage(DataStoragePath);


            public static AddressStorage StorageContainerInstance
            {
                get
                {
                    lock (LockObject)
                    {
                        return ContainerInstance;
                    }
                }
            }
        }
    }
}