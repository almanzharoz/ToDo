using System;

namespace Expo3.Model.Helpers
{
    public struct Price
    {
        public int Rubles { get; set; }
        public int Pennies { get; set; }

        public Price(int rubles, int pennies)
        {
            if(pennies < 0 || pennies > 99) throw new ArgumentException("Pennies should be in range between 0 and 99");
            if(rubles < 0) throw new ArgumentException("Rubles cannot be negative number");
            Rubles = rubles;
            Pennies = pennies;
        }
    }
}