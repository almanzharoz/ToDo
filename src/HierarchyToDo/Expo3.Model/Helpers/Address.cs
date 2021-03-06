﻿using Nest;

namespace Expo3.Model.Helpers
{
    public struct Address
    {
        public string City { get; set; }
        public string AddressString { get; set; }
        public GeoCoordinate Coordinates { get; set; }
    }
}