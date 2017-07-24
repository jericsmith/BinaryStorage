using System;
using System.Threading;
using BinaryStorage.AddressStorage;

namespace BinaryAddressStorage.Demo
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var instance = AddressStorage.StorageInstance;

      WriteAddresses(instance);
      ReadAddresses(instance);
      AddressStorage.StorageInstance.Dispose();
      Console.ReadKey();
    }

    private static void WriteAddresses(AddressStorage instance)
    {
      const string addressFormatLine1 = "{0} Dickinson";
      for (var i = 1; i < 2000; i++)
      {
        var line1 = string.Format(addressFormatLine1, 300 + i);
        var tp = new ThreadParm
        {
          Storage = instance,
          Line1 = line1
        };
        var t = new Thread(WriteAddress) {Name = $"Thread {i}"};
        t.Start(tp);
      }
    }

    private static void WriteAddress(object tp)
    {
        var parmObject = tp as ThreadParm;

      if (parmObject == null) return;

      var address = new Address
      {
        Line1 = parmObject.Line1,
        City = "Philadelphia",
        State = "PA",
        PostalCode = "19147-2500"
      };
      AddressStorage.StorageInstance.WriteAddress(address);
    }

    private static void ReadAddresses(object instance)
    {
        var storage = instance as AddressStorage;
      if (storage == null) return;
      var addresses = storage.GetAddresses();

      foreach (var addr in addresses)
        Console.WriteLine(addr.ToString());
    }

    private class ThreadParm
    {
      public AddressStorage Storage { get; set; }
      public string Line1 { get; set; }
    }
  }
}