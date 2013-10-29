using System;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategies
{
    public class CurrencyFactorsLoadedEventArgs : EventArgs
    {
        public CurrencyFactorsLoadedEventArgs(int reloadFactorsTimeout)
        {
            ReloadFactorsTimeout = reloadFactorsTimeout;
        }

        public int ReloadFactorsTimeout { get; set; }
    }
}