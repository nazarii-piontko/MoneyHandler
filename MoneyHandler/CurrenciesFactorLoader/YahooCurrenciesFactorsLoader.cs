using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MoneyHandler.CurrenciesFactorProvider;

namespace MoneyHandler.CurrenciesFactorLoader
{
    public class YahooCurrenciesFactorsLoader : ICurrenciesFactorsLoader
    {
        private WebClient _client = null;

        public CurrenciesFactorsPer1Usd LoadCurrenciesFactors()
        {
            if (_client != null)
                ThrowMoreThatOneConnectionError();

            string yahooData;

            try
            {
                using (_client = new WebClient())
                {
                    yahooData = _client.DownloadString(CreateUrlForLoad());
                    _client.Dispose();
                }
            }
            finally
            {
                _client = null;
            }

            return ParseLoadedData(yahooData);
        }

        public void LoadCurrenciesFactorsAsync(CurrenciesFactorsLoaderCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (_client != null)
                ThrowMoreThatOneConnectionError();

            _client = new WebClient();
            _client.DownloadStringCompleted += OnClientDownloadStringCompleted;
            _client.DownloadStringAsync(CreateUrlForLoad(), callback);
        }

        public void CancelAsync()
        {
            if (_client == null)
                return;

            try { _client.CancelAsync(); }
            catch { }
        }

        public void Dispose()
        {
            CancelAsync();
        }

        private void OnClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (_client == null)
                return;

            try
            {
                ((CurrenciesFactorsLoaderCallback) e.UserState)(
                    new CurrenciesFactorsLoaderCalbackState(ParseLoadedData(e.Result)
                                                            , e.Error
                                                            , e.Cancelled));
            }
            finally
            {
                _client.DownloadStringCompleted -= OnClientDownloadStringCompleted;
                _client.Dispose();
                _client = null;
            }
        }

        private CurrenciesFactorsPer1Usd ParseLoadedData(string yahooData)
        {
            CurrenciesFactorsPer1Usd factors = new CurrenciesFactorsPer1Usd();
            CsvReader reader = new CsvReader(new StringReader(yahooData), CreateCsvReaderConfiguration());

            while (reader.Read())
            {
                var symbol = reader.GetField(0);

                if (symbol.Length < 6)
                    continue;

                symbol = symbol.Substring(0, symbol.Length - 5).ToLower();

                Currency currency;

                try { currency = (Currency) Enum.Parse(typeof (Currency), symbol.ToUpper()); }
                catch { continue; }

                if (currency == Currency.UNKNOWN  || currency == Currency.USD)
                    continue;

                decimal factor;

                if (!Decimal.TryParse(reader.GetField(1), NumberStyles.Number, CultureInfo.InvariantCulture, out factor)
                    || factor <= 0m)
                    continue;

                factors[currency] = factor;
            }

            return factors;
        }

        private static Uri CreateUrlForLoad()
        {
            IEnumerable<Currency> currencies = Enum.GetValues(typeof(Currency)).OfType<Currency>();
            StringBuilder strBuilder = new StringBuilder();

            foreach (var currency in currencies)
            {
                if (currency == Currency.UNKNOWN || currency == Currency.USD)
                    continue;

                if (strBuilder.Length > 0)
                    strBuilder.Append(',');

                strBuilder.Append(currency.ToString().ToUpper());
                strBuilder.Append("USD=X");
            }

            return new Uri(String.Format("http://finance.yahoo.com/d/quotes.csv?s={0}&f=sl1d1t1c1ohgv&e=.csv", strBuilder));
        }

        private static CsvConfiguration CreateCsvReaderConfiguration()
        {
            return new CsvConfiguration
            {
                Delimiter = ',',
                Quote = '"',
                HasHeaderRecord = false
            };
        }

        private static void ThrowMoreThatOneConnectionError()
        {
            throw new ApplicationException("Cannot make more than one connection at the same time");
        }
    }
}