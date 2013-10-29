using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MoneyHandler.CurrenciesFactorProviders;
using MoneyHandler.CurrencyDescriptors;
using MoneyHandler.Properties;

namespace MoneyHandler
{
    /// <summary>
    /// Represents an instant in money: amount and currency.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Auto)]
    public struct Money : IComparable, IComparable<Money>, IEquatable<Money>, IFormattable, IConvertible, ISerializable, IXmlSerializable,
                          ICloneable
    {
        #region Private fields

        private const string AmountName = "Amount";
        private const string CurrencyName = "Currency";

        private decimal _amount;
        private Currency _currency;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoneyHandler.Money"/> structure to a specified amount and currency.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="currency">One of the enumeration values that indicates currency.</param>
        public Money(decimal amount, Currency currency)
        {
            _amount = amount;
            _currency = currency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoneyHandler.Money"/> structure to a specified amount and currency.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="currency">One of the enumeration values that indicates currency.</param>
        public Money(double amount, Currency currency)
            : this((decimal) amount, currency)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoneyHandler.Money"/> structure to a specified amount and currency.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="currency">One of the enumeration values that indicates currency.</param>
        public Money(long amount, Currency currency)
            : this((decimal)amount, currency)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoneyHandler.Money"/> structure to a specified amount of dollars.
        /// </summary>
        /// <param name="amount">The amount.</param>
        public Money(decimal amount)
        {
            _amount = amount;
            _currency = MoneyHandlerSettings.Instance.DefaultCurrency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoneyHandler.Money"/> structure to a specified amount of dollars.
        /// </summary>
        /// <param name="amount">The amount.</param>
        public Money(double amount)
            : this((decimal) amount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoneyHandler.Money"/> structure to a specified amount of dollars.
        /// </summary>
        /// <param name="amount">The amount.</param>
        public Money(long amount)
            : this((decimal) amount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoneyHandler.Money"/> structure to a zero amount and specific currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        public Money(Currency currency)
        {
            _amount = 0m;
            _currency = currency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoneyHandler.Money"/> structure to another money instance.
        /// </summary>
        /// <param name="other">The money.</param>
        public Money(Money other)
        {
            _amount = other._amount;
            _currency = other._currency;
        }

        private Money(SerializationInfo info, StreamingContext context)
        {
            _amount = info.GetDecimal(AmountName);
            _currency = (Currency) info.GetValue(CurrencyName, typeof (Currency));
        }

        #endregion

        #region Public methods and properties

        /// <summary>
        /// The amount of money.
        /// </summary>
        public decimal Amount
        {
            get { return _amount; }
        }

        /// <summary>
        /// The amount of money yhat round accourding currency SignificantDecimalDigits.
        /// </summary>
        public decimal RoundedAmount
        {
            get { return Math.Round(_amount, CurrencyDescriptor.SignificantDecimalDigits); }
        }

        /// <summary>
        /// The currency of money.
        /// </summary>
        public Currency Currency
        {
            get { return _currency; }
        }

        /// <summary>
        /// The currency descriptor of currency of money.
        /// </summary>
        public CurrencyDescriptor CurrencyDescriptor
        {
            get { return CurrencyDescriptor.GetDescriptor(_currency); }
        }

        /// <summary>
        /// Convert this instanse of money to <paramref name="targetCurrency"/> using <paramref name="factorProvider"/>.
        /// </summary>
        /// <param name="targetCurrency">The target currency.</param>
        /// <param name="factorProvider">The currencies factor provider.</param>
        /// <returns>The new instanse of money with <paramref name="targetCurrency"/>.</returns>
        public Money ConvertToCurrency(Currency targetCurrency, ICurrenciesFactorProvider factorProvider)
        {
            return _currency != targetCurrency
                       ? new Money(_amount*factorProvider.GetFactor(_currency, targetCurrency), targetCurrency)
                       : this;
        }

        /// <summary>
        /// Convert this instanse of money to <paramref name="targetCurrency"/> using default currencies factor provider.
        /// </summary>
        /// <param name="targetCurrency">The target currency.</param>
        /// <returns>The new instanse of money with <paramref name="targetCurrency"/>.</returns>
        public Money ConvertToCurrency(Currency targetCurrency)
        {
            return ConvertToCurrency(targetCurrency, MoneyHandlerSettings.Instance.FactorProvider);
        }

        public delegate decimal AllocateRoundStrategy(decimal amount
                                                      , decimal totalAmount, Currency currency
                                                      , int index, decimal[] ratios);

        public delegate decimal AllocateCorrectLastAmountStrategy(decimal amount
                                                                  , decimal totalAmount, Currency currency
                                                                  , decimal[] ratios, Money[] calculatedMoney);

        /// <summary>
        /// Split instance of money accourding to ratios and roundStrategy
        /// </summary>
        /// <param name="ratios">The ratios.</param>
        /// <param name="roundStrategy"></param>
        /// <param name="correctLastAmountStrategy"></param>
        /// <returns>Array of money</returns>
        public Money[] Allocate(decimal[] ratios, AllocateRoundStrategy roundStrategy, AllocateCorrectLastAmountStrategy correctLastAmountStrategy)
        {
            if (ratios == null)
                throw new ArgumentNullException("ratios");
            if (roundStrategy == null)
                throw new ArgumentNullException("roundStrategy");
            if (correctLastAmountStrategy == null)
                throw new ArgumentNullException("correctLastAmountStrategy");

            decimal total = ratios.Sum();
            decimal remainder = _amount;

            var result = new Money[ratios.Length];
            int len = result.Length - 1;

            for (int i = 0; i < len; ++i)
            {
                decimal amount = roundStrategy(_amount*ratios[i]/total, _amount, _currency, i, ratios);

                result[i] = new Money(amount, _currency);
                remainder -= amount;
            }

            result[len] = new Money(correctLastAmountStrategy(remainder, _amount, _currency, ratios, result), _currency);

            return result;
        }

        /// <summary>
        /// Split instance of money accourding to ratios and roundStrategy
        /// </summary>
        /// <param name="ratios">The ratios.</param>
        /// <returns>Array of money</returns>
        public Money[] Allocate(decimal[] ratios)
        {
            return Allocate(ratios
                            , (amount, totalAmount, currency, index, decimals) => Math.Round(amount, CurrencyDescriptor.GetDescriptor(currency).SignificantDecimalDigits)
                            , (amount, totalAmount, currency, decimals, money) => amount);
        }

        public string ToString(string format)
        {
            return ToString(format, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        #endregion

        #region Implementation of IComparable

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            if (!(obj is Money))
                ThrowInvalidArgumentTypeException("obj");

            return CompareTo((Money) obj);
        }

        #endregion

        #region Implementation of IComparable<in Money>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Money other)
        {
            other = CorrectCurrencyIfNeed(_currency, other);

            return _amount.CompareTo(other._amount);
        }

        #endregion

        #region Implementation of IEquatable<Money>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Money other)
        {
            return (this == other);
        }

        #endregion

        #region Implementation of IFormattable

        /// <summary>
        /// Formats the value of the current instance using the specified format.
        /// It's use String.Format:
        /// 0 - Rounded Amount
        /// 1 - Amount
        /// 2 - ISO currency name
        /// 3 - Currency symbol
        /// 4 - English name
        /// 5 - Native name
        /// </summary>
        /// <returns>
        /// The value of the current instance in the specified format.
        /// </returns>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable"/> implementation. </param><param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system. </param><filterpriority>2</filterpriority>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (String.IsNullOrEmpty(format))
                return ToString(formatProvider);

            var descriptor = CurrencyDescriptor.GetDescriptor(_currency);

            return String.Format(formatProvider, format, RoundedAmount, _amount, descriptor.IsoCode, descriptor.Symbol, descriptor.EnglishName, descriptor.NativeName);
        }

        #endregion

        #region Implementation of IConvertible

        /// <summary>
        /// Returns the <see cref="T:System.TypeCode"/> for this instance.
        /// </summary>
        /// <returns>
        /// The enumerated constant that is the <see cref="T:System.TypeCode"/> of the class or value type that implements this interface.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent Boolean value using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A Boolean value equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return _amount != 0m;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent Unicode character using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A Unicode character equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException("Cannot convert to 'System.Char'");
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 8-bit signed integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return (sbyte) _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 8-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return (byte) _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 16-bit signed integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return (short) _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 16-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return (ushort) _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 32-bit signed integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return (int) _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 32-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return (uint) _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 64-bit signed integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return (long) _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 64-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return (ulong) _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent single-precision floating-point number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A single-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return (float) _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent double-precision floating-point number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A double-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return (double) _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.Decimal"/> number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Decimal"/> number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return _amount;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.DateTime"/> using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.DateTime"/> instance equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException("Cannot convert to 'System.DateTime'");
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.String"/> using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> instance equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        public string ToString(IFormatProvider provider)
        {
            return String.Format(provider, "{0} {1}", RoundedAmount, _currency);
        }

        /// <summary>
        /// Converts the value of this instance to an <see cref="T:System.Object"/> of the specified <see cref="T:System.Type"/> that has an equivalent value, using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> instance of type <paramref name="conversionType"/> whose value is equivalent to the value of this instance.
        /// </returns>
        /// <param name="conversionType">The <see cref="T:System.Type"/> to which the value of this instance is converted. </param><param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param><filterpriority>2</filterpriority>
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof (Money))
                return this;
            throw new InvalidCastException(String.Format("Cannot convert to '{0}'", conversionType.FullName));
        }

        #endregion

        #region Implementation of ISerializable

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(AmountName, _amount);
            info.AddValue(CurrencyName, _currency);
        }

        #endregion

        #region Implementation of IXmlSerializable

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized. </param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            string strValue = reader.ReadString().Trim();

            if (strValue != String.Empty)
            {
                var money = Parse(strValue);

                _amount = money._amount;
                _currency = money._currency;
            }
            else
            {
                _amount = 0m;
                _currency = MoneyHandlerSettings.Instance.DefaultCurrency;
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized. </param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteString(ToString("{1} {2}", CultureInfo.InvariantCulture));
        }

        #endregion

        #region Implementation of ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return new Money(this);
        }

        #endregion

        #region Overriden

        public override int GetHashCode()
        {
// ReSharper disable NonReadonlyFieldInGetHashCode
            return _amount.GetHashCode() ^ _currency.GetHashCode();
// ReSharper restore NonReadonlyFieldInGetHashCode
        }

        public override bool Equals(object obj)
        {
            return (obj is Money) && Equals((Money) obj);
        }

        public override string ToString()
        {
            return ToString(System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        #endregion

        #region Operators

        public static bool operator ==(Money l, Money r)
        {
            return l._amount == r._amount
                   && l._currency == r._currency;
        }

        public static bool operator !=(Money l, Money r)
        {
            return !(l == r);
        }

        public static bool operator <(Money l, Money r)
        {
            r = CorrectCurrencyIfNeed(l._currency, r);

            return l._amount < r._amount;
        }

        public static bool operator >(Money l, Money r)
        {
            r = CorrectCurrencyIfNeed(l._currency, r);

            return l._amount > r._amount;
        }

        public static bool operator <=(Money l, Money r)
        {
            r = CorrectCurrencyIfNeed(l._currency, r);

            return l._amount <= r._amount;
        }

        public static bool operator >=(Money l, Money r)
        {
            r = CorrectCurrencyIfNeed(l._currency, r);

            return l._amount >= r._amount;
        }

        public static explicit operator decimal(Money l)
        {
            return l._amount;
        }

        public static explicit operator double(Money l)
        {
            return (double) l._amount;
        }

        public static explicit operator float(Money l)
        {
            return (float) l._amount;
        }

        public static explicit operator Currency(Money l)
        {
            return l._currency;
        }

        public static explicit operator Money(decimal l)
        {
            return new Money(l);
        }

        public static explicit operator Money(double l)
        {
            return new Money(l);
        }

        public static explicit operator Money(long l)
        {
            return new Money((decimal) l);
        }

        public static explicit operator Money(string str)
        {
            return Parse(str);
        }

        public static Money operator +(Money l, Money r)
        {
            r = CorrectCurrencyIfNeed(l._currency, r);

            return new Money(l._amount + r._amount, l._currency);
        }

        public static Money operator +(Money l, decimal r)
        {
            return new Money(l._amount + r, l._currency);
        }

        public static Money operator +(Money l, double r)
        {
            return new Money(l._amount + (decimal) r, l._currency);
        }

        public static Money operator +(Money l, long r)
        {
            return new Money(l._amount + r, l._currency);
        }

        public static Money operator +(decimal l, Money r)
        {
            return r + l;
        }

        public static Money operator +(double l, Money r)
        {
            return r + l;
        }

        public static Money operator +(long l, Money r)
        {
            return r + l;
        }

        public static Money operator -(Money l, Money r)
        {
            r = CorrectCurrencyIfNeed(l._currency, r);

            return new Money(l._amount - r._amount, l._currency);
        }

        public static Money operator -(Money l, decimal r)
        {
            return new Money(l._amount - r, l._currency);
        }

        public static Money operator -(Money l, double r)
        {
            return new Money(l._amount - (decimal) r, l._currency);
        }

        public static Money operator -(Money l, long r)
        {
            return new Money(l._amount - r, l._currency);
        }

        public static Money operator -(decimal l, Money r)
        {
            return r - l;
        }

        public static Money operator -(double l, Money r)
        {
            return r - l;
        }

        public static Money operator -(long l, Money r)
        {
            return r - l;
        }

        public static Money operator *(Money l, decimal r)
        {
            return new Money(l._amount*r, l._currency);
        }

        public static Money operator *(Money l, double r)
        {
            return new Money(l._amount*(decimal) r, l._currency);
        }

        public static Money operator *(Money l, long r)
        {
            return new Money(l._amount*r, l._currency);
        }

        public static Money operator *(decimal l, Money r)
        {
            return r*l;
        }

        public static Money operator *(double l, Money r)
        {
            return r*l;
        }

        public static Money operator *(long l, Money r)
        {
            return r*l;
        }

        public static Money operator /(Money l, decimal r)
        {
            return new Money(l._amount/r, l._currency);
        }

        public static Money operator /(Money l, double r)
        {
            return new Money(l._amount/(decimal) r, l._currency);
        }

        public static Money operator /(Money l, long r)
        {
            return new Money(l._amount/r, l._currency);
        }

        public static Money operator %(Money l, decimal r)
        {
            return new Money(l._amount%r, l._currency);
        }

        public static Money operator %(Money l, double r)
        {
            return new Money(l._amount%(decimal) r, l._currency);
        }

        public static Money operator %(Money l, long r)
        {
            return new Money(l._amount%r, l._currency);
        }

        #endregion

        #region Static

        /// <summary>
        /// Converts the specified string representation of money to its <see cref="T:MoneyHandler.Money"/> equivalent.
        /// </summary>
        public static Money Parse(string s)
        {
            return Parse(s, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Converts the specified string representation of money to its <see cref="T:MoneyHandler.Money"/> equivalent using the specified culture-specific format information.
        /// </summary>
        public static Money Parse(string s, IFormatProvider provider)
        {
            Money money;
            Exception ex;

            if (!TryParsePrivate(s, provider, out money, out ex))
            {
                if (ex == null)
                    ex = new ArgumentException(Resources.Str_Money_UnknownParseError, "s");
                throw ex;
            }
            return money;
        }

        public static bool TryParse(string s, out Money money)
        {
            return TryParse(s, System.Threading.Thread.CurrentThread.CurrentCulture, out money);
        }

        public static bool TryParse(string s, IFormatProvider provider, out Money money)
        {
            Exception ex;

            return TryParsePrivate(s, provider, out money, out ex);
        }

        private static bool TryParsePrivate(string s, IFormatProvider provider, out Money money, out Exception ex)
        {
            money = new Money();

            if (s == null)
            {
                ex = new ArgumentNullException("s");
                return false;
            }

            s = s.Trim();

            if (s.Length < 4)
            {
                ex = new FormatException("String too short");
                return false;
            }

            try
            {

                Currency currency;

                if (Char.IsLetter(s[0]))
                {
                    int lastIndex = 1;

                    for (; lastIndex < s.Length; ++lastIndex)
                    {
                        if (!Char.IsLetter(s[lastIndex]))
                            break;
                    }

                    currency = (Currency) Enum.Parse(typeof (Currency), s.Substring(0, lastIndex).ToUpper());
                    s = s.Substring(lastIndex);
                }
                else if (Char.IsLetter(s[s.Length - 1]))
                {
                    int startIndex = 1;

                    for (; startIndex >= 0; --startIndex)
                    {
                        if (!Char.IsLetter(s[startIndex]))
                            break;
                    }

                    currency = (Currency)Enum.Parse(typeof(Currency), s.Substring(startIndex + 1).ToUpper());
                    s = s.Substring(0, startIndex + 1);
                }
                else
                    currency = MoneyHandlerSettings.Instance.DefaultCurrency;

                decimal amount = Decimal.Parse(s, NumberStyles.Number, provider);

                money._amount = Math.Round(amount, CurrencyDescriptor.GetDescriptor(currency).SignificantDecimalDigits);
                money._currency = currency;
            }
            catch (Exception e)
            {
                ex = e;
                return false;
            }

            ex = null;
            return true;
        }

        #endregion

        #region Private

        private static void ThrowInvalidArgumentTypeException(string paramName)
        {
            throw new ArgumentException(Resources.Str_Money_ThrowInvalidArgumentTypeException, paramName);
        }

        private static Money CorrectCurrencyIfNeed(Currency baseCurrency, Money other)
        {
            if (baseCurrency == other._currency)
                return other;
            return other.ConvertToCurrency(baseCurrency);
        }

        #endregion
    }
}
