using System;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategies
{
    public class CurrencyFactorsLoadingErrorEventArgs : EventArgs
    {
        private readonly Exception _error;

        public CurrencyFactorsLoadingErrorEventArgs(Exception error, int reloadFactorsTimeout)
        {
            _error = error;
            ReloadFactorsTimeout = reloadFactorsTimeout;
        }

        public int ReloadFactorsTimeout { get; set; }

        public Exception Error
        {
            get { return _error; }
        }
    }
}