using System;
using MoneyHandler.CurrenciesFactorLoaders;
using MoneyHandler.CurrenciesFactorsUpdateStrategies;
using MoneyHandler.Extensions;

namespace MoneyHandler.Example
{
    class Program
    {
        static void Main()
        {
            //This is not nessesary configuration step for special settings.
            //MoneyHandlerSettings.Init(
            //    new MoneyHandlerSettings(
            //        new DefaultCurrenciesFactorsUpdateStrategy(
            //            new FileCachableCurrenciesFactorsLoader(
            //                new YahooCurrenciesFactorsLoader(), "Cache.csv")
            //                , isPreviousLoadSuccessful => isPreviousLoadSuccessful ? (6 * 60 * 60 * 1000) : 60 * 1000), Currency.USD));

            MoneyHandlerSettings.InitDefault(); // Not nessesary call
            Console.WriteLine("---------------------- YAHOO DATA ----------------------");
            PrintData();

            MoneyHandlerSettings.Init(new MoneyHandlerSettings(new CsvFileCurrenciesFactorsUpdateStrategy("currencies.csv"), Currency.USD));
            Console.WriteLine("---------------------- CSV DATA ----------------------");
            PrintData();

            MoneyHandlerSettings.Init(
                    new MoneyHandlerSettings(
                        new DefaultCurrenciesFactorsUpdateStrategy(
                            new FileCachableCurrenciesFactorsLoader(new YahooCurrenciesFactorsLoader(),
                                                                    "currencies_cache.csv"),
                            isPreviousLoadSuccessful => isPreviousLoadSuccessful ? (5 * 1000) : 1000), Currency.USD));
            Console.WriteLine("---------------------- CACHEBLE DATA ----------------------");
            PrintData();

            Console.ReadKey();
        }

        private static void PrintData()
        {
            Console.WriteLine("1 EUR in USD  = {0}", 1m.Euros().ToDollars());
            Console.WriteLine("1 Dolars = {0}", new Money(1, Currency.USD));
            Console.WriteLine("1 Dolars In Euro = {0}", 1m.Dollars().ToEuros());
            Console.WriteLine("1 Dolars In Yeans = {0}", 1m.Dollars().ToJapaneseYens());
            Console.WriteLine("1 Eur In Yeans = {0}", 1m.Euros().ToJapaneseYens());
            Console.WriteLine("2 Dolars + 3 Euro = {0}", (2m.Dollars() + 3m.Euros()).ToString("{0} {4}"));
            Console.WriteLine("1 Dolar in UAH = {0}", 1m.Dollars().ToHryvnas());
        }
    }
}
