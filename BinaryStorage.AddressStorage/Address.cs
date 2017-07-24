using System.Text;
using BinaryStorage.Abstractions;

namespace BinaryStorage.AddressStorage
{
    public class Address : IAddress
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        public bool IsValid()
        {
            return (null != Line1 && null != City && null != State && null != PostalCode);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Line1);
            if (!string.IsNullOrEmpty(Line2)) sb.AppendLine(Line2);
            sb.AppendFormat("{0}, {1} {2}\n", City, State, PostalCode);
            return sb.ToString();
        }
    }
}