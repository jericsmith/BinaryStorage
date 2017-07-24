namespace BinaryStorage.Abstractions
{
    public interface IAddress
    {
        string City { get; set; }
        string Line1 { get; set; }
        string Line2 { get; set; }
        string PostalCode { get; set; }
        string State { get; set; }

        bool IsValid();
        string ToString();
    }
}