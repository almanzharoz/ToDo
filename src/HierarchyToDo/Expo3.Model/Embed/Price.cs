using System;

namespace Expo3.Model.Embed
{
    public struct Price
    {
        private int _rubles;
        private int _pennies;

        public int Rubles
        {
            get => _rubles;
            set
            {
                if(value < 0) throw new ArgumentException("Rubles cannot be negative number");
                _rubles = value;
            }
        }

        public int Pennies
        {
            get => _pennies;
            set
            {
                if(value < 0 || value > 99) throw new ArgumentException("Pennies should be in range between 0 and 99");
                _pennies = value;
            }
        }
    }
}