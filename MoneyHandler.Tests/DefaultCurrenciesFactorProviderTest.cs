using MoneyHandler.CurrenciesFactorProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoneyHandler.Tests
{
    /// <summary>
    ///This is a test class for DefaultCurrenciesFactorProviderTest and is intended
    ///to contain all DefaultCurrenciesFactorProviderTest Unit Tests
    ///</summary>
    [TestClass]
    public class DefaultCurrenciesFactorProviderTest
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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
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


        /// <summary>
        ///A test for GetFactor
        ///</summary>
        [TestMethod]
        public void GetFactorTest()
        {
            var factorsPer1Usd = new CurrenciesFactorsPer1Usd();

            factorsPer1Usd[Currency.EUR] = 1.4m;
            factorsPer1Usd[Currency.JPY] = 2.8m;

            var target = new DefaultCurrenciesFactorProvider(factorsPer1Usd);

            Assert.AreEqual(2m, target.GetFactor(Currency.EUR, Currency.JPY));
            Assert.AreEqual(1m/2m, target.GetFactor(Currency.JPY, Currency.EUR));
            Assert.AreEqual(2.8m, target.GetFactor(Currency.USD, Currency.JPY));
            Assert.AreEqual(1m, target.GetFactor(Currency.USD, Currency.USD));
            Assert.AreEqual(1m, target.GetFactor(Currency.JPY, Currency.JPY));
            Assert.AreEqual(1m, target.GetFactor(Currency.USD, Currency.UNKNOWN));
        }
    }
}
