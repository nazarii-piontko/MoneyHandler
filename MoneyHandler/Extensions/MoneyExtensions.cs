#if !EXTENSION_METHODS_NOT_SUPPORTED

namespace MoneyHandler.Extensions
{
    public static class MoneyExtensions
    {
        /// <summary>
        /// To USD
        /// </summary>
        public static Money ToDollars(this Money money)
        {
            return money.ConvertToCurrency(Currency.USD);
        }

        /// <summary>
        /// To EUR
        /// </summary>
        public static Money ToEuros(this Money money)
        {
            return money.ConvertToCurrency(Currency.EUR);
        }

        /// <summary>
        /// To GBP
        /// </summary>
        public static Money ToPounds(this Money money)
        {
            return money.ConvertToCurrency(Currency.GBP);
        }

        /// <summary>
        /// To JPY
        /// </summary>
        public static Money ToJapaneseYens(this Money money)
        {
            return money.ConvertToCurrency(Currency.JPY);
        }

        /// <summary>
        /// To AUD
        /// </summary>
        public static Money ToAustralianDollars(this Money money)
        {
            return money.ConvertToCurrency(Currency.AUD);
        }

        /// <summary>
        /// To CHF
        /// </summary>
        public static Money ToSwissFrancs(this Money money)
        {
            return money.ConvertToCurrency(Currency.CHF);
        }

        /// <summary>
        /// To CAD
        /// </summary>
        public static Money ToCanadianDollars(this Money money)
        {
            return money.ConvertToCurrency(Currency.CAD);
        }

        /// <summary>
        /// To RUB
        /// </summary>
        public static Money ToRubles(this Money money)
        {
            return money.ConvertToCurrency(Currency.RUB);
        }

        /// <summary>
        /// To UAH
        /// </summary>
        public static Money ToHryvnas(this Money money)
        {
            return money.ConvertToCurrency(Currency.UAH);
        }
    }
}

#endif