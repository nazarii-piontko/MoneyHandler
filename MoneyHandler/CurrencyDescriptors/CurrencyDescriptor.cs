using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Xml;

namespace MoneyHandler.CurrencyDescriptors
{
    public sealed class CurrencyDescriptor
    {
        private static readonly ReadOnlyCollection<Int32> DefaultGroupSizes = new ReadOnlyCollection<Int32>(new List<Int32> {3});

        private static readonly object DescriptorsLocker = new object();
        private static CurrencyDescriptor[] _descriptors;

        private static CurrencyDescriptor[] Descriptors
        {
            get
            {
                if (_descriptors == null)
                {
                    lock (DescriptorsLocker)
                    {
                        if (Interlocked.CompareExchange(ref _descriptors, null, null) == null)
                            CreateDescriptors();
                    }
                }
                return _descriptors;
            }
        }

        private static void CreateDescriptors()
        {
            var descriptors = new CurrencyDescriptor[CurrenciesCount];

            var doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.Currencies);

            foreach (XmlNode node in doc.ChildNodes[1].ChildNodes)
            {
                if (node.Attributes == null)
                    continue;

                Currency currency;

                try { currency = (Currency) Enum.Parse(typeof (Currency), node.Attributes[0].Value.Trim().ToUpper()); }
                catch { continue; }

                try
                {
                    var descriptor = new CurrencyDescriptor
                                         {
                                             Currency = currency,
                                             IsoCode = currency.ToString(),
                                             EnglishName = node.ChildNodes[0].InnerText.Trim(),
                                             NativeName = node.ChildNodes[1].InnerText.Trim(),
                                             Symbol = String.Concat(SplitNumberSequence(node.ChildNodes[2]).Select(s => Char.ConvertFromUtf32(Int32.Parse(s))).ToArray()),
                                             SignificantDecimalDigits = Int32.Parse(node.ChildNodes[3].InnerText),
                                             DecimalSeparator = GetCharFromString(node.ChildNodes[4].InnerText.Trim()),
                                             GroupSeparator = GetCharFromString(node.ChildNodes[5].InnerText.Trim()),
                                             GroupSizes =
                                                 new ReadOnlyCollection<Int32>(
                                                 (from s in SplitNumberSequence(node.ChildNodes[6]) select Int32.Parse(s)).
                                                     ToList()),
                                             PositivePattern = Int32.Parse(node.ChildNodes[7].InnerText),
                                             NegativePattern = Int32.Parse(node.ChildNodes[8].InnerText)
                                         };

                    descriptors[(int) currency] = descriptor;
                }
                catch
                { }
            }

            for (int i = 0; i < descriptors.Length; ++i)
            {
                if (descriptors[i] == null)
                {
                    var currency = (Currency) i;

                    descriptors[i] = new CurrencyDescriptor
                                         {
                                             Currency = currency,
                                             IsoCode = currency.ToString(),
                                             EnglishName = currency.ToString(),
                                             SignificantDecimalDigits = 2,
                                             DecimalSeparator = '.',
                                             GroupSeparator = ',',
                                             GroupSizes = DefaultGroupSizes
                                         };
                }
            }

            Interlocked.CompareExchange(ref _descriptors, descriptors, null);
        }

        private static IEnumerable<String> SplitNumberSequence(XmlNode node)
        {
            return node.InnerText.Trim().Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
        }

        private static char GetCharFromString(string s)
        {
            if (s.Length == 0)
                return ' ';
            return s[0];
        }

        public static IEnumerable<CurrencyDescriptor> GetDescriptors()
        {
            return Descriptors;
        }

        public static CurrencyDescriptor GetDescriptor(Currency currency)
        {
            return Descriptors[(int) currency];
        }

        public static int CurrenciesCount
        {
            get { return Enum.GetValues(typeof (Currency)).Length; }
        }

        private CurrencyDescriptor()
        { }

        public Currency Currency { get; private set; }
        public string IsoCode { get; private set; }
        public string EnglishName { get; private set; }
        public string NativeName { get; private set; }
        public string Symbol { get; private set; }

        public char DecimalSeparator { get; private set; }
        public int SignificantDecimalDigits { get; private set; }

        public char GroupSeparator { get; private set; }
        public IList<Int32> GroupSizes { get; private set; }

        public int PositivePattern { get; private set; }
        public int NegativePattern { get; private set; }
    }
}