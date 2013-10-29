using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.Tests
{
    /// <summary>
    ///This is a test class for CurrenciesFactorsPer1UsdTest and is intended
    ///to contain all CurrenciesFactorsPer1UsdTest Unit Tests
    ///</summary>
    [TestClass]
    public class CurrenciesFactorsPer1UsdTest
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
        ///A test for CurrenciesFactorsPer1UnitInUsd Constructor
        ///</summary>
        [TestMethod]
        public void CurrenciesFactorsPer1UsdConstructorTest()
        {
            var target = new CurrenciesFactorsPer1UnitInUsd();

            Assert.AreEqual(target[Currency.UNKNOWN], 1m);
            Assert.AreEqual(target[Currency.USD], 1m);
            Assert.AreEqual(target[Currency.EUR], 1m);
            Assert.AreEqual(target[Currency.JPY], 1m);
        }

        /// <summary>
        ///A test for Clone
        ///</summary>
        [TestMethod]
        public void CloneTest()
        {
            var target = new CurrenciesFactorsPer1UnitInUsd();

            target[Currency.EUR] = 1.4m;
            target[Currency.JPY] = 2m;

            var actual = (CurrenciesFactorsPer1UnitInUsd) target.Clone();

            Assert.AreEqual(target[Currency.EUR], actual[Currency.EUR]);
            Assert.AreEqual(target[Currency.JPY], actual[Currency.JPY]);
        }

        /// <summary>
        ///A test for Count
        ///</summary>
        [TestMethod]
        public void CountTest()
        {
            var target = new CurrenciesFactorsPer1UnitInUsd();

            Assert.AreEqual(target.Count, Enum.GetValues(typeof (Currency)).Length);
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod]
        public void ItemTest()
        {
            var target = new CurrenciesFactorsPer1UnitInUsd();

            target[Currency.EUR] = 1.4m;

            Assert.AreEqual(target[Currency.EUR], 1.4m);
        }
    }
}
