using System;
using System.Collections.Generic;

namespace BinaryStorage.Abstractions
{
    public interface IAddressStorage : IDisposable
    {
        void WriteAddress(IAddress address);
        IEnumerable<IAddress> GetAddresses();
    }
}