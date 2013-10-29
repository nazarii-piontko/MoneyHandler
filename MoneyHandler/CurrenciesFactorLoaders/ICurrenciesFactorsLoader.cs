using System;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.CurrenciesFactorLoaders
{
    public interface ICurrenciesFactorsLoader : IDisposable
    {
        CurrenciesFactorsPer1UnitInUsd LoadCurrenciesFactors();

        void LoadCurrenciesFactorsAsync(CurrenciesFactorsLoaderCallback callback);
        void CancelAsync();
    }

    public delegate void CurrenciesFactorsLoaderCallback(CurrenciesFactorsLoaderCalbackState state);

    public class CurrenciesFactorsLoaderCalbackState
    {
        private readonly CurrenciesFactorsPer1UnitInUsd _factors;
        private readonly Exception _error;
        private readonly bool _isCancelled;

        public CurrenciesFactorsLoaderCalbackState(CurrenciesFactorsPer1UnitInUsd factors, Exception error, bool isCancelled)
        {
            _factors = factors;
            _error = error;
            _isCancelled = isCancelled;
        }

        public bool IsCancelled
        {
            get { return _isCancelled; }
        }

        public Exception Error
        {
            get { return _error; }
        }

        public CurrenciesFactorsPer1UnitInUsd Factors
        {
            get { return _factors; }
        }
    }
}