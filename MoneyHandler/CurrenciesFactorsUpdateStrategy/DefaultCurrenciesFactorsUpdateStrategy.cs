using System;
using System.Timers;
using MoneyHandler.CurrenciesFactorLoader;
using MoneyHandler.CurrenciesFactorProvider;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategy
{
    public class DefaultCurrenciesFactorsUpdateStrategy : ICurrenciesFactorsUpdateStrategy
    {
        public const int MinTimerTimeout = 100;

        private readonly ICurrenciesFactorsLoader _factorsLoader;
        private DefaultCurrenciesFactorProvider _factorsProvider;

        private int _updateTimeout = 12 * 3600 * 100;//default value every 12 hours
        private Timer _timer = null;

        private bool _isDisposed = false;

        public event EventHandler<CurrencyFactorsLoadingErrorEventArgs> FactorsLoadingError;
        public event EventHandler<CurrencyFactorsLoadedEventArgs> FactorsLoaded;

        public DefaultCurrenciesFactorsUpdateStrategy(ICurrenciesFactorsLoader factorsLoader)
        {
            if (factorsLoader == null)
                throw new ArgumentNullException("factorsLoader");

            _factorsLoader = factorsLoader;
        }

        public int UpdateTimeout
        {
            get { return _updateTimeout; }
            set
            {
                if (_updateTimeout == value)
                    return;
                if (_isDisposed)
                    return;

                _timer.Interval = _updateTimeout;
                _updateTimeout = value;
            }
        }

        public ICurrenciesFactorProvider CreateAndInitProvider()
        {
            CheckDisposedFlag();

            if (_factorsProvider == null)
                _factorsProvider = new DefaultCurrenciesFactorProvider();

            InitTimer();
            UpdateFactorsSync();

            return _factorsProvider;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            if (_timer != null)
            {
                try { _timer.Stop(); }
                catch { }
                _timer.Dispose();
                _timer = null;
            }

            try { _factorsLoader.CancelAsync(); }
            catch { }
            _factorsLoader.Dispose();

            _isDisposed = true;
        }

        private void UpdateFactorsSync()
        {
            try
            {
                _factorsProvider.UpdateFactors(_factorsLoader.LoadCurrenciesFactors());
            }
            catch (Exception ex)
            {
                HandleUpdateFactorsError(ex);
                return;
            }
            HandleUpdateFactorsLoaded();
        }

        private void InitTimer()
        {
            if (_timer != null)
                return;

            _timer = new Timer {AutoReset = true};
            _timer.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_isDisposed)
                return;

            UpdateFactorsAsync();
        }

        private void UpdateFactorsAsync()
        {
            try
            {
                _factorsLoader.LoadCurrenciesFactorsAsync(CurrenciesFactorsLoaderCallback);
            }
            catch (Exception ex)
            {
                HandleUpdateFactorsError(ex);
                return;
            }
            HandleUpdateFactorsLoaded();
        }

        private void CurrenciesFactorsLoaderCallback(CurrenciesFactorsLoaderCalbackState state)
        {
            if (_isDisposed)
                return;
            if (state.IsCancelled)
                return;

            if (state.Error != null)
                HandleUpdateFactorsError(state.Error);
            else
            {
                _factorsProvider.UpdateFactors(state.Factors);
                HandleUpdateFactorsLoaded();
            }
        }

        private void HandleUpdateFactorsLoaded()
        {
            var args = new CurrencyFactorsLoadedEventArgs(_updateTimeout);

            try
            {
                if (FactorsLoaded != null)
                    FactorsLoaded(this, args);
            }
            finally
            {
                StartTimer(args.ReloadFactorsTimeout);
            }
        }

        private void HandleUpdateFactorsError(Exception ex)
        {
            var args = new CurrencyFactorsLoadingErrorEventArgs(ex, _updateTimeout);

            try
            {
                if (FactorsLoadingError != null)
                    FactorsLoadingError(this, args);
            }
            finally
            {
                StartTimer(args.ReloadFactorsTimeout);
            }
        }

        private void StartTimer(int timeout)
        {
            _timer.Interval = Math.Max(timeout, MinTimerTimeout);
            _timer.Start();
        }

        private void CheckDisposedFlag()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("DefaultCurrenciesFactorsUpdateStrategy");
        }
    }

    public class CurrencyFactorsLoadedEventArgs : EventArgs
    {
        public CurrencyFactorsLoadedEventArgs(int reloadFactorsTimeout)
        {
            ReloadFactorsTimeout = reloadFactorsTimeout;
        }

        public int ReloadFactorsTimeout { get; set; }
    }

    public class CurrencyFactorsLoadingErrorEventArgs : EventArgs
    {
        private readonly Exception _error;

        public CurrencyFactorsLoadingErrorEventArgs(Exception error, int reloadFactorsTimeout)
        {
            _error = error;
            ReloadFactorsTimeout = reloadFactorsTimeout;
        }

        public int ReloadFactorsTimeout { get; set; }

        public Exception Error
        {
            get { return _error; }
        }
    }
}