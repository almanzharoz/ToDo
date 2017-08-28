
namespace BeeFee.Model.Embed
{
    public struct Address
    {
        public string City { get; }
        public string AddressString { get; }
        //public GeoCoordinate Coordinates { get; set; }

	    public Address(string city, string address)
	    {
		    City = city;
		    AddressString = address;
	    }
    }
}