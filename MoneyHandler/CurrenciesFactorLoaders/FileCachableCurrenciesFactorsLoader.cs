using System;
using System.IO;

namespace MoneyHandler.CurrenciesFactorLoaders
{
    /// <summary>
    /// Used as cache file
    /// </summary>
    public abstract class FileCachableCurrenciesFactorsLoaderBase : CachableCurrenciesFactorsLoader
    {
        protected FileCachableCurrenciesFactorsLoaderBase(ICurrenciesFactorsLoader loader) 
            : base(loader)
        {
            SetFactorsData = SetFactorsData2Cache;
            GetFactorsData = GetFactorsData2Cache;
        }

        private void SetFactorsData2Cache(byte[] data)
        {
            if (data != null)
                File.WriteAllBytes(GetCacheFilePath(), data);
        }

        private byte[] GetFactorsData2Cache()
        {
            var filePath = GetCacheFilePath();

            return !File.Exists(filePath) ? null : File.ReadAllBytes(filePath);
        }

        protected abstract string GetCacheFilePath();
    }

    public sealed class FileCachableCurrenciesFactorsLoader : FileCachableCurrenciesFactorsLoaderBase
    {
        private readonly string _cacheFilePath;

        public FileCachableCurrenciesFactorsLoader(ICurrenciesFactorsLoader loader, string cacheFilePath) 
            : base(loader)
        {
            if (String.IsNullOrEmpty(cacheFilePath))
                throw new ArgumentNullException("cacheFilePath");

            _cacheFilePath = cacheFilePath;
        }

        protected override string GetCacheFilePath()
        {
            return _cacheFilePath;
        }
    }
}