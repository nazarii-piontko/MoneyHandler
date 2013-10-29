namespace MoneyHandler.CurrenciesFactorProviders
{
    /// <summary>
    /// Provide currencies factor that allow convert one currency to another.
    /// </summary>
    public interface ICurrenciesFactorProvider
    {
        /// <summary>
        /// Get currencies factor: how many cost 1 amount of <paramref name="baseCurrency"/> in <paramref name="targetCurrency"/>.
        /// </summary>
        /// <param name="baseCurrency">The base currency.</param>
        /// <param name="targetCurrency">The target currency.</param>
        /// <returns>The factor value.</returns>
        decimal GetFactor(Currency baseCurrency, Currency targetCurrency);
    }
}