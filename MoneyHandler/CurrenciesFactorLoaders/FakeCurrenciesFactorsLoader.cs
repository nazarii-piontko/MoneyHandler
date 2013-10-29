using System;
using System.Threading;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.CurrenciesFactorLoaders
{
    public class FakeCurrenciesFactorsLoader : ICurrenciesFactorsLoader
    {
        private readonly CurrenciesFactorsPer1UnitInUsd _factors;

        public bool ThrowException { get; set; }

        public FakeCurrenciesFactorsLoader(CurrenciesFactorsPer1UnitInUsd factors)
        {
            _factors = factors;
        }

        ~FakeCurrenciesFactorsLoader()
        {
            Dispose(false);
        }

        public CurrenciesFactorsPer1UnitInUsd LoadCurrenciesFactors()
        {
            if (ThrowException)
                throw new ApplicationException("Fake");

            return _factors;
        }

        public void LoadCurrenciesFactorsAsync(CurrenciesFactorsLoaderCallback callback)
        {
            ThreadPool.QueueUserWorkItem(
                t =>
                callback(ThrowException
                             ? new CurrenciesFactorsLoaderCalbackState(null, new ApplicationException("Fake"), false)
                             : new CurrenciesFactorsLoaderCalbackState(_factors, null, false)));
        }

        public void CancelAsync()
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