using System.Text;
using MoneyHandler.CurrenciesFactorLoaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.Tests
{
    
    
    /// <summary>
    ///This is a test class for CachableCurrenciesFactorsLoaderTest and is intended
    ///to contain all CachableCurrenciesFactorsLoaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CachableCurrenciesFactorsLoaderTest
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

        [TestMethod]
        public void LoadCurrenciesFactorsTestWithoutException()
        {
            var target = CreateTarget(false);
            var actual = target.LoadCurrenciesFactors();
            
            Assert.AreEqual(1.4m, actual[Currency.EUR]);
        }

        [TestMethod]
        public void LoadCurrenciesFactorsTestWitException()
        {
            var target = CreateTarget(true);
            var actual = target.LoadCurrenciesFactors();

            Assert.AreEqual(1.1m, actual[Currency.EUR]);
        }

        private CachableCurrenciesFactorsLoader CreateTarget(bool throwException)
        {
            var loader = new FakeCurrenciesFactorsLoader(new CurrenciesFactorsPer1UnitInUsd());

            loader.LoadCurrenciesFactors()[Currency.EUR] = 1.4m;

            var target = new CachableCurrenciesFactorsLoader(loader)
                             {
                                 GetFactorsData = GetFactorsData, 
                                 SetFactorsData = SetFactorsData
                             };

            loader.ThrowException = throwException;

            return target;
        }

        private byte[] GetFactorsData()
        {
            return Encoding.UTF8.GetBytes("EUR,1.1");
        }

        private void SetFactorsData(byte[] data)
        { }
    }
}
