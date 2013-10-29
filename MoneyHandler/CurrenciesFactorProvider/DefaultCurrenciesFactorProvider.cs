namespace MoneyHandler.CurrenciesFactorProvider
{
    public class DefaultCurrenciesFactorProvider : ICurrenciesFactorProvider
    {
        private CurrenciesFactorsPer1Usd _factors;

        public DefaultCurrenciesFactorProvider()
            : this(new CurrenciesFactorsPer1Usd())
        { }

        public DefaultCurrenciesFactorProvider(CurrenciesFactorsPer1Usd factors)
        {
            UpdateFactors(factors);
        }

        public void UpdateFactors(CurrenciesFactorsPer1Usd factors)
        {
            _factors = factors;
        }

        public CurrenciesFactorsPer1Usd GetFactorsCopy()
        {
            return (CurrenciesFactorsPer1Usd) _factors.Clone();
        }

        public decimal GetFactor(Currency baseCurrency, Currency targetCurrency)
        {
            if (baseCurrency == Currency.UNKNOWN || targetCurrency == Currency.UNKNOWN)
                return 1m;

            if (baseCurrency == Currency.USD)
                return _factors[targetCurrency];
            
            return _factors[targetCurrency] / _factors[baseCurrency];
        }
    }
}