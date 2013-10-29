using MoneyHandler.CurrenciesFactorProvider;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategy
{
    public class FakeLoadCurrenciesFactorsUpdateStrategy : ICurrenciesFactorsUpdateStrategy
    {
        public ICurrenciesFactorProvider CreateAndInitProvider()
        {
            return new DefaultCurrenciesFactorProvider(new CurrenciesFactorsPer1Usd());
        }

        public void Dispose()
        { }
    }
}