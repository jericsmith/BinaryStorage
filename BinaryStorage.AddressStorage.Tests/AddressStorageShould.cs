using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinaryStorage.Abstractions;
using NUnit.Framework;

namespace BinaryStorage.AddressStorage.Tests
{
    [TestFixture]
    public class AddressStorageShould
    {
        private static string DataStoragePath;
        [SetUp]
        public void Setup()
        {
            DataStoragePath = ConfigurationManager.AppSettings["dataStoragePath"];
            // just in case
            DeleteTestFile();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTestFile();
        }

        private static void DeleteTestFile()
        {

            if (File.Exists(DataStoragePath))
            {
                File.Delete(DataStoragePath);
            }
        }
        [Test]
        public void Be_thread_safe()
        {
            using (AddressStorage.StorageInstance)
            {
                var t1 = Task.Factory.StartNew(WriteAddresses);
                var t2 = Task.Factory.StartNew(WriteAddresses);

                Task.WaitAll(t1, t2);

                Assert.AreEqual(4000, AddressStorage.StorageInstance.GetAddresses().Count());
            }
        }

        [Test]
        public void Return_a_nicely_formatted_string_for_display()
        {
            using (AddressStorage.StorageInstance)
            {
                IAddress address = new Address
                {
                    Line1 = "line 1",
                    Line2 = "line 2",
                    City = "city",
                    State = "state",
                    PostalCode = "11111"
                };
                var display = address.ToString();
                int start = 0;
                var count = 0;
                while (true)
                {
                    var ix = display.IndexOf("\n", start);
                    if (ix > -1)
                    {
                        start = ix + 1;
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                Assert.AreEqual(3,count);
            }
        }

        [Test]
        public void Throw_exception_when_address_is_not_valid()
        {
            using (AddressStorage.StorageInstance)
            {
                IAddress address = new Address
                {
                    Line1 = "line 1",
                    Line2 = "line 2",
                    City = "city",
                    State = "state"
                };
                Assert.Throws<InvalidDataException>(() => AddressStorage.StorageInstance.WriteAddress(address));
            }
        }
        private static void WriteAddresses()
        {
            const string addressFormatLine1 = "{0} Dickinson";
            for (var i = 1; i <= 2000; i++)
            {
                var line1 = string.Format(addressFormatLine1, 300 + i);
                var tp = new ThreadParm
                {
                    Line1 = line1
                };
                var t = new Thread(WriteAddress);
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

        private class ThreadParm
        {
            public string Line1 { get; set; }
        }
    }
}
