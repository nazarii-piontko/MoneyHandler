using System.Collections.Generic;
using MoneyHandler.CurrenciesFactorProviders;
using MoneyHandler.CurrenciesFactorsUpdateStrategies;
using MoneyHandler.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoneyHandler.Tests
{
    /// <summary>
    ///This is a test class for MoneyConvertionTest and is intended
    ///to contain all MoneyConvertionTest Unit Tests
    ///</summary>
    [TestClass]
    public class MoneyConvertionTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class

        private static CurrenciesFactorsPer1UnitInUsd Factors;

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            var strategy =
                new FakeLoadCurrenciesFactorsUpdateStrategy(new List<KeyValuePair<Currency, decimal>>
                                                                {
                                                                    new KeyValuePair<Currency, decimal>(Currency.EUR, 1.4m),
                                                                    new KeyValuePair<Currency, decimal>(Currency.JPY, 1.5m),
                                                                    new KeyValuePair<Currency, decimal>(Currency.UAH, 0.2m),
                                                                    new KeyValuePair<Currency, decimal>(Currency.RUB, 0.11m),
                                                                    new KeyValuePair<Currency, decimal>(Currency.CHF, 2m)
                                                                });

            Factors = ((DefaultCurrenciesFactorProvider)strategy.CreateAndInitProvider()).GetFactorsCopy();

            MoneyHandlerSettings.Init(new MoneyHandlerSettings(strategy, Currency.USD));
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void TestEUR()
        {
            const Currency currency = Currency.EUR;

            CheckDollarConvertion(currency);
        }

        [TestMethod]
        public void TestUAH()
        {
            const Currency currency = Currency.UAH;

            CheckDollarConvertion(currency);
        }

        [TestMethod]
        public void TestUSD()
        {
            const Currency currency = Currency.USD;

            CheckDollarConvertion(currency);
        }

        [TestMethod]
        public void TestEUR2AUH()
        {
            CheckGeneralConvertion(Currency.EUR, Currency.UAH);
        }

        [TestMethod]
        public void TestUAH2EUR()
        {
            CheckGeneralConvertion(Currency.UAH, Currency.EUR);
        }

        private static void CheckDollarConvertion(Currency currency)
        {
            var money = 1m.Dollars();

            Assert.AreEqual(money.ConvertToCurrency(currency).Amount, 1m/Factors[currency]);
            Assert.AreEqual(money.ConvertToCurrency(currency).Currency, currency);

            money = 1m.Crs(currency);

            Assert.AreEqual(money.ToDollars().Amount, Factors[currency]);
            Assert.AreEqual(money.ToDollars().Currency, Currency.USD);
        }

        private static void CheckGeneralConvertion(Currency currency1, Currency currency2)
        {
            var money = 1m.Crs(currency1);

            Assert.AreEqual(money.ConvertToCurrency(currency2).Amount, Factors[currency1]/Factors[currency2]);
            Assert.AreEqual(money.ConvertToCurrency(currency2).Currency, currency2);
        }
    }
}
