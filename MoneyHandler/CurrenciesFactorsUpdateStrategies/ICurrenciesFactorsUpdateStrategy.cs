using System;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategies
{
    public interface ICurrenciesFactorsUpdateStrategy : IDisposable
    {
        ICurrenciesFactorProvider CreateAndInitProvider();

        /// <summary>
        /// Force update currency factors asyncroniosly
        /// </summary>
        void ForceUpdate();
    }
}