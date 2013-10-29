using System;
using MoneyHandler.CurrenciesFactorProvider;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategy
{
    public interface ICurrenciesFactorsUpdateStrategy : IDisposable
    {
        ICurrenciesFactorProvider CreateAndInitProvider();
    }
}