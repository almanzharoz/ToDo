using System;

namespace Expo3.Model.Embed
{
    public struct Price
    {
        private int _rubles;
        private byte _pennies;

        public int Rubles
        {
            get => _rubles;
            set
            {
                if(value < 0) throw new ArgumentException("Rubles cannot be negative number");
                _rubles = value;
            }
        }

        public byte Pennies
        {
            get => _pennies;
            set
            {
                if(value > 99) throw new ArgumentException("Pennies should be in range between 0 and 99");
                _pennies = value;
            }
        }

        public Price(int rubles, byte pennies)
        {
            _rubles = 0;
            _pennies = 0;
            Rubles = rubles;
            Pennies = pennies;
        }
    }
}