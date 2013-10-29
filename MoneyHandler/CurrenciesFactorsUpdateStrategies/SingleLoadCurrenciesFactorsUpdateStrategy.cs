using System;
using MoneyHandler.CurrenciesFactorLoaders;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategies
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

        ~SingleLoadCurrenciesFactorsUpdateStrategy()
        {
            Dispose(false);
        }

        public ICurrenciesFactorProvider CreateAndInitProvider()
        {
            ForceUpdate();

            return _factorsProvider;
        }

        public void ForceUpdate()
        {
            if (_factorsProvider == null)
                _factorsProvider = new DefaultCurrenciesFactorProvider();

            _factorsProvider.UpdateFactors(_factorsLoader.LoadCurrenciesFactors());
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            _factorsLoader.Dispose();
            if (disposing)
                GC.SuppressFinalize(this);
        }
    }
}