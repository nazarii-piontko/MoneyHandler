using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MoneyHandler.CurrenciesFactorProvider
{
    public class CurrenciesFactorsPer1Usd : IEnumerable<KeyValuePair<Currency, decimal>>, ICloneable
    {
        private readonly decimal[] _factors;

        public CurrenciesFactorsPer1Usd()
        {
            _factors = new decimal[Enum.GetValues(typeof (Currency)).Length];
            for (int i = 0; i < _factors.Length; ++i)
                _factors[i] = 1m;
        }

        private CurrenciesFactorsPer1Usd(decimal[] factors)
        {
            _factors = factors;
        }

        public decimal this[Currency currency]
        {
            get { return _factors[(int) currency]; }
            set
            {
                if (currency == Currency.UNKNOWN)
                    throw new ArgumentException("Cannot set factor for UNKNOWN currency", "currency");
                if (currency == Currency.USD)
                    throw new ArgumentException("Cannot set factor for Usd currency because it always equal 1m", "currency");
                if (value <= 0m)
                    throw new ArgumentOutOfRangeException("value", "Value must greater that zero");

                _factors[(int) currency] = value;
            }
        }

        public int Count
        {
            get { return _factors.Length; }
        }

        public IEnumerator<KeyValuePair<Currency, decimal>> GetEnumerator()
        {
            return _factors.Select((t, i) => new KeyValuePair<Currency, decimal>((Currency) i, t)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object Clone()
        {
            return new CurrenciesFactorsPer1Usd((decimal[]) _factors.Clone());
        }
    }
}