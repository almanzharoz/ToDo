using Nest;

namespace Expo3.Model
{
    public class Address
    {
        public string AddressString { get; set; }
        public GeoCoordinate Coordinates { get; set; }
    }
}