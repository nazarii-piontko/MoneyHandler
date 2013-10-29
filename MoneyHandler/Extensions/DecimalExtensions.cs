#if !EXTENSION_METHODS_NOT_SUPPORTED

namespace MoneyHandler.Extensions
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// To USD
        /// </summary>
        public static Money Dollars(this decimal amount)
        {
            return new Money(amount, Currency.USD);
        }

        /// <summary>
        /// To EUR
        /// </summary>
        public static Money Euros(this decimal amount)
        {
            return new Money(amount, Currency.EUR);
        }

        /// <summary>
        /// To GBP
        /// </summary>
        public static Money Pounds(this decimal amount)
        {
            return new Money(amount, Currency.GBP);
        }

        /// <summary>
        /// To JPY
        /// </summary>
        public static Money JapanYens(this decimal amount)
        {
            return new Money(amount, Currency.JPY);
        }

        /// <summary>
        /// To AUD
        /// </summary>
        public static Money AustralianDollars(this decimal amount)
        {
            return new Money(amount, Currency.AUD);
        }

        /// <summary>
        /// To CHF
        /// </summary>
        public static Money SwissFrancs(this decimal amount)
        {
            return new Money(amount, Currency.CHF);
        }

        /// <summary>
        /// To CAD
        /// </summary>
        public static Money CanadianDollars(this decimal amount)
        {
            return new Money(amount, Currency.CAD);
        }

        /// <summary>
        /// To RUB
        /// </summary>
        public static Money Rubles(this decimal amount)
        {
            return new Money(amount, Currency.RUB);
        }

        /// <summary>
        /// To UAH
        /// </summary>
        public static Money Hryvnas(this decimal amount)
        {
            return new Money(amount, Currency.UAH);
        }
    }
}

#endif