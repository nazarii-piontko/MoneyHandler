using System;
using System.IO;
using System.Text;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.CurrenciesFactorLoaders
{
    public class CachableCurrenciesFactorsLoader : ICurrenciesFactorsLoader
    {
        public delegate void SetFactorsData2CacheDelegate(byte[] data);
        public delegate byte[] GetFactorsDataFromCacheDelegate();

        private readonly ICurrenciesFactorsLoader _loader;

        public SetFactorsData2CacheDelegate SetFactorsData { get; set; }
        public GetFactorsDataFromCacheDelegate GetFactorsData { get; set; }

        public event EventHandler<ExceptionEventArgs> Exception;

        public CachableCurrenciesFactorsLoader(ICurrenciesFactorsLoader loader)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");
            _loader = loader;
        }

        ~CachableCurrenciesFactorsLoader()
        {
            Dispose(false);
        }

        public CurrenciesFactorsPer1UnitInUsd LoadCurrenciesFactors()
        {
            try
            {
                var factors = _loader.LoadCurrenciesFactors();

                try { SetCurrencyFactors2Cache(factors); }
                catch (Exception ex) { FireException(new ExceptionEventArgs(ex)); }

                return factors;
            }
            catch (Exception ex)
            {
                FireException(new ExceptionEventArgs(ex));
                return LoadCurrenciesFactorsFromCache();
            }
        }

        public void LoadCurrenciesFactorsAsync(CurrenciesFactorsLoaderCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _loader.LoadCurrenciesFactorsAsync(state =>
                                                   {
                                                       if (state.IsCancelled)
                                                       {
                                                           callback(state);
                                                           return;
                                                       }
                                                       if (state.Error == null)
                                                       {
                                                           try { SetCurrencyFactors2Cache(state.Factors); }
                                                           catch (Exception ex) { FireException(new ExceptionEventArgs(ex)); }
                                                           callback(state);
                                                           return;
                                                       }

                                                       FireException(new ExceptionEventArgs(state.Error));

                                                       CurrenciesFactorsPer1UnitInUsd factors = null;
                                                       Exception exception = null;
                                                       
                                                       try { factors = LoadCurrenciesFactorsFromCache(); }
                                                       catch (Exception ex) { exception = ex; }

                                                       callback(new CurrenciesFactorsLoaderCalbackState(factors, exception, false));
                                                   });
        }

        public void CancelAsync()
        {
            _loader.CancelAsync();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            _loader.Dispose();
            if (disposing)
                GC.SuppressFinalize(this);
        }

        private void FireException(ExceptionEventArgs e)
        {
            var handler = Exception;
            if (handler != null)
                handler(this, e);
        }

        private CurrenciesFactorsPer1UnitInUsd LoadCurrenciesFactorsFromCache()
        {
            var getFactorsData = GetFactorsData;

            if (getFactorsData == null)
            {
                FireException(new ExceptionEventArgs(new ApplicationException("Delegate GetFactorsData == null")));
                return new CurrenciesFactorsPer1UnitInUsd();
            }

            var data = getFactorsData();

            if (data == null)
            {
                FireException(new ExceptionEventArgs(new ApplicationException("There is no data in cache")));
                return new CurrenciesFactorsPer1UnitInUsd();
            }

            using (var reader = new StreamReader(new MemoryStream(data), Encoding.UTF8, true))
                return CsvFileCurrenciesFactorsLoader.ParseFileData(reader);
        }

        private void SetCurrencyFactors2Cache(CurrenciesFactorsPer1UnitInUsd factors)
        {
            if (factors == null)
                return;

            var setCacheData = SetFactorsData;

            if (setCacheData == null)
                return;

            setCacheData(Encoding.UTF8.GetBytes(CsvFileCurrenciesFactorsLoader.CreateFileData(factors)));
        }
    }
}