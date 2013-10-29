using System;
using MoneyHandler.CurrencyDescriptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoneyHandler;

namespace MoneyHandler.Tests
{
    
    
    /// <summary>
    ///This is a test class for CurrencyDescriptorTest and is intended
    ///to contain all CurrencyDescriptorTest Unit Tests
    ///</summary>
    [TestClass]
    public class CurrencyDescriptorTest
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
        ///A test for GetDescriptor
        ///</summary>
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(CurrencyDescriptor.CurrenciesCount, Enum.GetValues(typeof (Currency)).Length);

            Assert.AreEqual(CurrencyDescriptor.GetDescriptor(Currency.EUR).IsoCode, "EUR");
            Assert.AreEqual(CurrencyDescriptor.GetDescriptor(Currency.EUR).NativeName, "Euro");
            Assert.AreEqual(CurrencyDescriptor.GetDescriptor(Currency.EUR).SignificantDecimalDigits, 2);
        }
    }
}
