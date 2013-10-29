using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MoneyHandler.CurrenciesFactorProviders
{
    public class CurrenciesFactorsPer1UnitInUsd : IEnumerable<KeyValuePair<Currency, decimal>>, ICloneable
    {
        private readonly decimal[] _factors;

        public CurrenciesFactorsPer1UnitInUsd()
        {
            _factors = new decimal[Enum.GetValues(typeof (Currency)).Length];
            for (int i = 0; i < _factors.Length; ++i)
                _factors[i] = 1m;
        }

        private CurrenciesFactorsPer1UnitInUsd(decimal[] factors)
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
            for (int i = 0; i < _factors.Length; ++i)
            {
                if (i == (int) Currency.UNKNOWN || i == (int) Currency.USD)
                    continue;

                yield return new KeyValuePair<Currency, decimal>((Currency) i, _factors[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object Clone()
        {
            return new CurrenciesFactorsPer1UnitInUsd((decimal[]) _factors.Clone());
        }
    }
}