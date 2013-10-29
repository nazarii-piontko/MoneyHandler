using System;
using MoneyHandler.CurrenciesFactorLoaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MoneyHandler.Tests
{
    /// <summary>
    ///This is a test class for CsvFileCurrenciesFactorsLoaderTest and is intended
    ///to contain all CsvFileCurrenciesFactorsLoaderTest Unit Tests
    ///</summary>
    [TestClass]
    public class CsvFileCurrenciesFactorsLoaderTest
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
        ///A test for ParseFileData
        ///</summary>
        [TestMethod]
        [DeploymentItem("MoneyHandler.dll")]
        public void ParseFileDataTest()
        {
            TextReader fileReader = new StringReader(
                "CURRENCY,FACTOR" + Environment.NewLine +
                "EUR,1.4" + Environment.NewLine +
                "AUD,1.5\r\n" + Environment.NewLine +
                "JPY,2\n" + Environment.NewLine +
                "UAH,0.2");

            var actual = CsvFileCurrenciesFactorsLoader_Accessor.ParseFileData(fileReader);

            Assert.AreEqual(1.4m, actual[Currency.EUR]);
            Assert.AreEqual(1.5m, actual[Currency.AUD]);
            Assert.AreEqual(2, actual[Currency.JPY]);
            Assert.AreEqual(0.2m, actual[Currency.UAH]);
        }
    }
}
