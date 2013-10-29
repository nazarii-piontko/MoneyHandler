namespace MoneyHandler.CurrenciesFactorProviders
{
    public class DefaultCurrenciesFactorProvider : ICurrenciesFactorProvider
    {
        private CurrenciesFactorsPer1UnitInUsd _factors;

        public DefaultCurrenciesFactorProvider()
            : this(new CurrenciesFactorsPer1UnitInUsd())
        { }

        public DefaultCurrenciesFactorProvider(CurrenciesFactorsPer1UnitInUsd factors)
        {
            UpdateFactors(factors);
        }

        public void UpdateFactors(CurrenciesFactorsPer1UnitInUsd factors)
        {
            _factors = factors;
        }

        public CurrenciesFactorsPer1UnitInUsd GetFactorsCopy()
        {
            return (CurrenciesFactorsPer1UnitInUsd) _factors.Clone();
        }

        public decimal GetFactor(Currency baseCurrency, Currency targetCurrency)
        {
            if (baseCurrency == Currency.UNKNOWN || targetCurrency == Currency.UNKNOWN)
                return 1m;

            return _factors[baseCurrency]/_factors[targetCurrency];
        }
    }
}