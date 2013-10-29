using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.CurrenciesFactorLoaders
{
    /// <summary>
    /// Load CSV files where every roid in following format (header row is not nessesary)
    /// 
    /// CURRENCY,CURRENCY_FACTOR_PER_ONE_USD
    /// 
    /// for example: EUR,1.4
    /// </summary>
    public sealed class CsvFileCurrenciesFactorsLoader : ICurrenciesFactorsLoader
    {
        private readonly string _filePath;

        public CsvFileCurrenciesFactorsLoader(string filePath)
        {
            _filePath = filePath;
        }

        ~CsvFileCurrenciesFactorsLoader()
        {
            Dispose(false);
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public CurrenciesFactorsPer1UnitInUsd LoadCurrenciesFactors()
        {
            return LoadFactorsFromFile();
        }

        public void LoadCurrenciesFactorsAsync(CurrenciesFactorsLoaderCallback callback)
        {
            CurrenciesFactorsPer1UnitInUsd factors = null;
            Exception exception = null;

            try
            {
                factors = LoadFactorsFromFile();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            ThreadPool.QueueUserWorkItem(
                t => callback(new CurrenciesFactorsLoaderCalbackState(factors, exception, false)));
        }

        public void CancelAsync()
        { }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            GC.SuppressFinalize(this);
        }

        private CurrenciesFactorsPer1UnitInUsd LoadFactorsFromFile()
        {
            if (!File.Exists(_filePath))
                return new CurrenciesFactorsPer1UnitInUsd();

            using (var reader = new StreamReader(_filePath, Encoding.UTF8, true))
                return ParseFileData(reader);
        }

        public static CurrenciesFactorsPer1UnitInUsd ParseFileData(TextReader fileReader)
        {
            var factors = new CurrenciesFactorsPer1UnitInUsd();
            var reader = new CsvReader(fileReader, CreateCsvReaderConfiguration());

            while (reader.Read())
            {
                var symbol = reader.GetField(0);

                Currency currency;

                try { currency = (Currency) Enum.Parse(typeof (Currency), symbol.ToUpper()); }
                catch { continue; }

                if (currency == Currency.UNKNOWN || currency == Currency.USD)
                    continue;

                decimal factor;

                if (!Decimal.TryParse(reader.GetField(1), NumberStyles.Number, CultureInfo.InvariantCulture, out factor)
                    || factor <= 0m)
                    continue;

                factors[currency] = factor;
            }

            return factors;
        }

        public static string CreateFileData(CurrenciesFactorsPer1UnitInUsd factors)
        {
            var builder = new StringBuilder();

            foreach (var factor in factors)
            {
                if (builder.Length > 0)
                    builder.AppendLine();
                builder.AppendFormat("{0},{1}", factor.Key, factor.Value.ToString(CultureInfo.InvariantCulture));
            }

            return builder.ToString();
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
    }
}