using System;
using System.Collections.Generic;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategies
{
    public sealed class FakeLoadCurrenciesFactorsUpdateStrategy : ICurrenciesFactorsUpdateStrategy
    {
        private readonly CurrenciesFactorsPer1UnitInUsd _factors;

        public FakeLoadCurrenciesFactorsUpdateStrategy()
        {
            _factors = new CurrenciesFactorsPer1UnitInUsd();
        }

        public FakeLoadCurrenciesFactorsUpdateStrategy(IEnumerable<KeyValuePair<Currency, decimal>> factors)
            : this()
        {
            foreach (var pair in factors)
            {
                if (pair.Key == Currency.UNKNOWN 
                    || pair.Key == Currency.USD
                    || pair.Value <= 0.0m)
                    continue;

                _factors[pair.Key] = pair.Value;
            }
        }

        ~FakeLoadCurrenciesFactorsUpdateStrategy()
        {
            Dispose(false);
        }

        public ICurrenciesFactorProvider CreateAndInitProvider()
        {
            return new DefaultCurrenciesFactorProvider(_factors);
        }

        public void ForceUpdate()
        { }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                GC.SuppressFinalize(this);
        }
    }
}