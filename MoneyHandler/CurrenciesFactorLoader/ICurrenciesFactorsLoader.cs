using System;
using MoneyHandler.CurrenciesFactorProvider;

namespace MoneyHandler.CurrenciesFactorLoader
{
    public interface ICurrenciesFactorsLoader : IDisposable
    {
        CurrenciesFactorsPer1Usd LoadCurrenciesFactors();

        void LoadCurrenciesFactorsAsync(CurrenciesFactorsLoaderCallback callback);
        void CancelAsync();
    }

    public delegate void CurrenciesFactorsLoaderCallback(CurrenciesFactorsLoaderCalbackState state);

    public class CurrenciesFactorsLoaderCalbackState
    {
        private readonly CurrenciesFactorsPer1Usd _factors;
        private readonly Exception _error;
        private readonly bool _isCancelled;

        public CurrenciesFactorsLoaderCalbackState(CurrenciesFactorsPer1Usd factors, Exception error, bool isCancelled)
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

        public CurrenciesFactorsPer1Usd Factors
        {
            get { return _factors; }
        }
    }
}