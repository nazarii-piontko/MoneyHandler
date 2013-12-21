using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System;
using MoneyHandler.CurrenciesFactorsUpdateStrategies;
using MoneyHandler.Extensions;

namespace MoneyHandler.Tests
{
    ///<summary>
    /// This is a test class for MoneyTest and is intended
    /// to contain all MoneyTest Unit Tests
    ///</summary>
    [TestClass]
    public class MoneyTest
    {
        private decimal _amount;
        private Currency _currency;
        private Money _money;

        private decimal _amount1;
        private Currency _currency1;
        private Money _money1;

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
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            MoneyHandlerSettings.Init(new MoneyHandlerSettings(new FakeLoadCurrenciesFactorsUpdateStrategy(), Currency.USD));
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize]
        public void MyTestInitialize()
        {
            _amount = 1257.13m;
            _currency = Currency.USD;
            _money = new Money(_amount, _currency);

            _amount1 = 244.43m;
            _currency1 = Currency.EUR;
            _money1 = new Money(_amount1, _currency1);
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for Money Constructor
        ///</summary>
        [TestMethod]
        public void MoneyConstructorTestForDecimalAndCurrency()
        {   
            Assert.AreEqual(_money.Amount, _amount);
            Assert.AreEqual(_money.Currency, _currency);
        }

        /// <summary>
        ///A test for Money Constructor
        ///</summary>
        [TestMethod]
        public void MoneyConstructorTestForDoubleAndCurrency()
        {
            const double amount = 13.5;
            const Currency currency = Currency.EUR;

            var target = new Money(amount, currency);

            Assert.AreEqual((double) target.Amount, amount);
            Assert.AreEqual(target.Currency, currency);
        }

        /// <summary>
        ///A test for Money Constructor
        ///</summary>
        [TestMethod]
        public void MoneyConstructorTestforDouble()
        {
            const double amount = 7.8;

            var target = new Money(amount);

            Assert.AreEqual((double) target.Amount, amount);
            Assert.AreEqual(target.Currency, Currency.USD);
        }

        /// <summary>
        ///A test for Money Constructor
        ///</summary>
        [TestMethod]
        public void MoneyConstructorTestForCurrency()
        {
            const Currency currency = Currency.EUR;

            var target = new Money(currency);
            
            Assert.AreEqual(target.Amount, 0m);
            Assert.AreEqual(target.Currency, currency);
        }

        /// <summary>
        ///A test for Money Constructor
        ///</summary>
        [TestMethod]
        public void MoneyConstructorTestForDecimal()
        {
            const decimal amount = 2334425.7872m;

            var target = new Money(amount);
            
            Assert.AreEqual(target.Amount, amount);
            Assert.AreEqual(target.Currency, Currency.USD);
        }

        /// <summary>
        ///A test for Money Constructor
        ///</summary>
        [TestMethod]
        public void MoneyConstructorTestForCopy()
        {
            var target = new Money(_money);

            Assert.AreEqual(target.Amount, _amount);
            Assert.AreEqual(target.Currency, _currency);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod]
        public void EqualsTestForEqualObject()
        {
            object obj = new Money(_amount, _currency);
            
            const bool expected = true;
            bool actual = _money.Equals(obj);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod]
        public void EqualsTestForNotEqualObject()
        {
            bool res = _money.Equals("Fiction");

            Assert.AreEqual(false, res);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod]
        public void EqualsTestForEqualMoney()
        {
            var other = new Money(_amount, _currency);

            const bool expected = true;
            bool actual = _money.Equals(other);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod]
        public void EqualsTestForNotEqualMoney()
        {
            var other = new Money(_amount, Currency.EUR);

            const bool expected = false;
            bool actual = _money.Equals(other);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod]
        public void OpEqualityTest()
        {
            var other = new Money(_amount, _currency);

            const bool expected = true;
            bool actual = _money == other;

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod]
        public void OpInequalityTest()
        {
            var other = new Money(_amount, _currency);

            const bool expected = false;
            bool actual = _money != other;

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod]
        public void GetHashCodeTest()
        {
            int hashCode = _money.GetHashCode();

            Assert.AreEqual(_amount.GetHashCode() ^ _currency.GetHashCode(), hashCode);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OpAdditionTestForDecimalAndMoney()
        {
            Money actual = _amount1 + _money;

            Assert.AreEqual(new Money(_amount1 + _money.Amount, _money.Currency), actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OpAdditionTestForDoubleAndMonet()
        {
            Money actual = (double)_amount1 + _money;

            Assert.AreEqual(new Money(_amount1 + _money.Amount, _money.Currency), actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OpAdditionTestForMoneyAndLong()
        {
            Money actual = _money + (long)_amount1;

            Assert.AreEqual(new Money((long)_amount1 + _money.Amount, _money.Currency), actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OpAdditionTestForMoneyAndDecimal()
        {
            Money actual = _money + _amount1;

            Assert.AreEqual(new Money(_amount1 + _money.Amount, _money.Currency), actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OpAdditionTestForMoneyAndDouble()
        {
            Money actual = _money + (double)_amount1;

            Assert.AreEqual(new Money(_amount1 + _money.Amount, _money.Currency), actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OpAdditionTestForMoneyAndMoney()
        {
            Money actual = _money + _money1;

            Assert.AreEqual(new Money(_money.Amount + _money1.ConvertToCurrency(_money.Currency).Amount, _money.Currency), actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OpAdditionTestForLongAndMoney()
        {
            Money actual = (long)_amount1 + _money;

            Assert.AreEqual(new Money((long)_amount1 + _money.Amount, _money.Currency), actual);
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod]
        public void OpDivisionTestForMoneyAndDouble()
        {
            Money actual = _money / (double)_amount1;

            Assert.AreEqual(new Money(_money.Amount / _amount1, _money.Currency), actual);
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod]
        public void OpDivisionTestForMoneyAndLong()
        {
            Money actual = _money / (long)_amount1;

            Assert.AreEqual(new Money(_money.Amount / (long)_amount1, _money.Currency), actual);
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod]
        public void OpDivisionTestForMoneyAndDecimal()
        {
            Money actual = _money / _amount1;

            Assert.AreEqual(new Money(_money.Amount / _amount1, _money.Currency), actual);
        }

        /// <summary>
        ///A test for op_Explicit
        ///</summary>
        [TestMethod]
        public void OpExplicitTestForMoneyToFloat()
        {
            Assert.AreEqual((float)_amount, (float) (_money));
        }

        /// <summary>
        ///A test for op_Explicit
        ///</summary>
        [TestMethod]
        public void OpExplicitTestForMoneyToDouble()
        {
            Assert.AreEqual((double)_amount, (double) (_money));
        }

        /// <summary>
        ///A test for op_Explicit
        ///</summary>
        [TestMethod]
        public void OpExplicitTestForMoneyToDecimal()
        {
            Assert.AreEqual(_amount, (Decimal)(_money));
        }

        /// <summary>
        ///A test for op_Explicit
        ///</summary>
        [TestMethod]
        public void OpExplicitTestForMoneyToCurrency()
        {
            Assert.AreEqual(_currency, (Currency) _money);
        }

        /// <summary>
        ///A test for op_Explicit
        ///</summary>
        [TestMethod]
        public void OpExplicitTestForLongToMoney()
        {
            const long l = 30;
            var expected = new Money((decimal)l);
            var actual = ((Money)(l));

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Explicit
        ///</summary>
        [TestMethod]
        public void OpExplicitTestForDoubleToMoney()
        {
            Assert.AreEqual(new Money(_amount), (Money) (double) _amount);
        }

        /// <summary>
        ///A test for op_Explicit
        ///</summary>
        [TestMethod]
        public void OpExplicitTestForDecimalToMoney()
        {
            Assert.AreEqual(new Money(_amount), (Money) _amount);
        }

        /// <summary>
        ///A test for op_GreaterThan
        ///</summary>
        [TestMethod]
        public void OpGreaterThanTest()
        {
            var l = new Money(12);
            var r = new Money(12);

            const bool expected = false;
            bool actual = (l > r);
            
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_GreaterThanOrEqual
        ///</summary>
        [TestMethod]
        public void OpGreaterThanOrEqualTest()
        {
            var l = new Money(12);
            var r = new Money(12);

            const bool expected = true;
            bool actual = (l >= r);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_LessThan
        ///</summary>
        [TestMethod]
        public void OpLessThanTest()
        {
            var l = new Money(11);
            var r = new Money(12);

            const bool expected = true;
            bool actual = (l < r);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_LessThanOrEqual
        ///</summary>
        [TestMethod]
        public void OpLessThanOrEqualTest()
        {
            var l = new Money(12);
            var r = new Money(12);

            const bool expected = true;
            bool actual = (l <= r);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod]
        public void OpMultiplyTestForMoneyAndDecimal()
        {
            Assert.AreEqual(new Money(_amount*_amount1, _currency), _money*_amount1);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod]
        public void OpSubtractionTestForMoneyAndDecimal()
        {
            Assert.AreEqual(new Money(_amount - _amount1, _currency), _money - _amount1);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod]
        public void OpSubtractionTestForMoneyAndMoney()
        {
            Money actual = _money - _money1;

            Assert.AreEqual(new Money(_money.Amount - _money1.ConvertToCurrency(_money.Currency).Amount, _money.Currency), actual);
        }

        /// <summary>
        ///A test for Allocate
        ///</summary>
        [TestMethod]
        public void AllocateTest()
        {
            var target = new Money(100);
            var ratios = new [] {0.2m, 0.3m, 0.25m, 0.25m};
            var expected = new[] { (Money)20, (Money)30, (Money)25, (Money)25 };

            Money[] actual = target.Allocate(ratios);

            for (int i = 0; i < ratios.Length; ++i)
                Assert.AreEqual(expected[i], actual[i]);
        }

        /// <summary>
        ///A test for Allocate
        ///</summary>
        [TestMethod]
        public void AllocateTest1()
        {
            var target = new Money(100);
            var ratios = new[] { 0.2m };
            var expected = new[] { (Money)100 };

            Money[] actual = target.Allocate(ratios);

            for (int i = 0; i < ratios.Length; ++i)
                Assert.AreEqual(expected[i], actual[i]);
        }

        /// <summary>
        ///A test for Allocate
        ///</summary>
        [TestMethod]
        public void ParseTest()
        {
            Assert.AreEqual(Money.Parse("18 EUR"), 18m.Euros());
            Assert.AreEqual(Money.Parse("17EUR"), 17m.Euros());
            Assert.AreEqual(Money.Parse("EUR 16"), 16m.Euros());
            Assert.AreEqual(Money.Parse("EUR15"), 15m.Euros());
            Assert.AreEqual(Money.Parse("eur 13"), 13m.Euros());
            Assert.AreEqual(Money.Parse("13$"), 13m.Dollars());
            Assert.AreEqual(Money.Parse("13  $"), 13m.Dollars());
            Assert.AreEqual(Money.Parse("$13"), 13m.Dollars());
            Assert.AreEqual(Money.Parse("$  13"), 13m.Dollars());
        }

        /// <summary>
        ///A test for Money Constructor
        ///</summary>
        [TestMethod]
        [DeploymentItem("MoneyHandler.dll")]
        public void MoneyConstructorSerializationInfoTest()
        {
            const decimal amount = 234.7m;
            const Currency currency = Currency.EUR;

            var info = new SerializationInfo(typeof(Money), new FormatterConverter());
            var context = new StreamingContext();

            info.AddValue("Amount", amount);
            info.AddValue("Currency", currency);

            var target = new Money_Accessor(info, context);

            Assert.AreEqual(target.Amount, amount);
            Assert.AreEqual(target.Currency, currency);
        }
    }
}
