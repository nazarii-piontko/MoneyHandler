using System;
using System.IO;
using System.Security.Permissions;
using MoneyHandler.CurrenciesFactorLoaders;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategies
{
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    public sealed class CsvFileCurrenciesFactorsUpdateStrategy : ICurrenciesFactorsUpdateStrategy
    {
        private readonly CsvFileCurrenciesFactorsLoader _factorsLoader;
        private DefaultCurrenciesFactorProvider _factorsProvider;

        private readonly FileSystemWatcher _fileWatcher = new FileSystemWatcher();

        public event EventHandler<CurrencyFactorsLoadingErrorEventArgs> FactorsLoadingError;
        public event EventHandler<CurrencyFactorsLoadedEventArgs> FactorsLoaded;

        public CsvFileCurrenciesFactorsUpdateStrategy(string filePath)
        {
            _factorsLoader = new CsvFileCurrenciesFactorsLoader(filePath);
        }

        ~CsvFileCurrenciesFactorsUpdateStrategy()
        {
            Dispose(false);
        }

        public ICurrenciesFactorProvider CreateAndInitProvider()
        {
            if (_factorsProvider == null)
                _factorsProvider = new DefaultCurrenciesFactorProvider();

            InitFileWatcher();
            UpdateFactors();

            return _factorsProvider;
        }

        public void ForceUpdate()
        {
            UpdateFactors();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { _fileWatcher.EnableRaisingEvents = false; }
                catch { }
                _fileWatcher.Dispose();

                GC.SuppressFinalize(this);
            }

            try
            { _factorsLoader.CancelAsync(); }
            catch
            { }
            _factorsLoader.Dispose();
        }

        private void UpdateFactors()
        {
            try
            {
                _factorsProvider.UpdateFactors(_factorsLoader.LoadCurrenciesFactors());
            }
            catch (Exception ex)
            {
                FireFactorsLoadingError(new CurrencyFactorsLoadingErrorEventArgs(ex, Int32.MaxValue));
                return;
            }
            FireFactorsLoaded(new CurrencyFactorsLoadedEventArgs(Int32.MaxValue));
        }

        private void InitFileWatcher()
        {
            string filePath = _factorsLoader.FilePath;

            if (!Path.IsPathRooted(filePath))
                filePath = Path.GetFullPath(filePath);

            _fileWatcher.Path = Path.GetDirectoryName(filePath);
            _fileWatcher.IncludeSubdirectories = false;
            _fileWatcher.Filter = Path.GetFileName(filePath);
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;

            _fileWatcher.Changed += FileWatcherChanged;

            _fileWatcher.EnableRaisingEvents = true;
        }

        private void FileWatcherChanged(object sender, FileSystemEventArgs e)
        {
            UpdateFactors();
        }

        private void FireFactorsLoadingError(CurrencyFactorsLoadingErrorEventArgs e)
        {
            var handler = FactorsLoadingError;
            if (handler != null)
                handler(this, e);
        }

        private void FireFactorsLoaded(CurrencyFactorsLoadedEventArgs e)
        {
            var handler = FactorsLoaded;
            if (handler != null)
                handler(this, e);
        }
    }
}