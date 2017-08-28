
using Newtonsoft.Json;

namespace BeeFee.Model.Embed
{
    public struct Address
    {
		[JsonProperty]
        public string City { get; private set; }
		[JsonProperty]
        public string AddressString { get; private set; }
        //public GeoCoordinate Coordinates { get; set; }

	    public Address(string city, string address)
	    {
		    City = city;
		    AddressString = address;
	    }
    }
}