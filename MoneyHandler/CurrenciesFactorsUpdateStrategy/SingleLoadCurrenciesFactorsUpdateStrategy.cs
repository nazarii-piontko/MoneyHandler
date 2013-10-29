using System;
using MoneyHandler.CurrenciesFactorLoader;
using MoneyHandler.CurrenciesFactorProvider;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategy
{
    public class SingleLoadCurrenciesFactorsUpdateStrategy : ICurrenciesFactorsUpdateStrategy
    {
        private readonly ICurrenciesFactorsLoader _factorsLoader;
        private DefaultCurrenciesFactorProvider _factorsProvider;

        public SingleLoadCurrenciesFactorsUpdateStrategy(ICurrenciesFactorsLoader factorsLoader)
        {
            if (factorsLoader == null)
                throw new ArgumentNullException("factorsLoader");

            _factorsLoader = factorsLoader;
        }

        public ICurrenciesFactorProvider CreateAndInitProvider()
        {
            if (_factorsProvider == null)
                _factorsProvider = new DefaultCurrenciesFactorProvider();

            _factorsProvider.UpdateFactors(_factorsLoader.LoadCurrenciesFactors());

            return _factorsProvider;
        }

        public void Dispose()
        {
            _factorsLoader.Dispose();
        }
    }
}