using System;
using System.Timers;
using MoneyHandler.CurrenciesFactorLoaders;
using MoneyHandler.CurrenciesFactorProviders;

namespace MoneyHandler.CurrenciesFactorsUpdateStrategies
{
    public sealed class DefaultCurrenciesFactorsUpdateStrategy : ICurrenciesFactorsUpdateStrategy
    {
        public delegate int TimerScheduleCallback(bool isPreviousLoadSuccessful);

        private readonly ICurrenciesFactorsLoader _factorsLoader;
        private readonly TimerScheduleCallback _scheduleCallback;

        private DefaultCurrenciesFactorProvider _factorsProvider;
        private readonly Timer _timer = new Timer();
        private bool _isDisposed;

        public event EventHandler<CurrencyFactorsLoadingErrorEventArgs> FactorsLoadingError;
        public event EventHandler<CurrencyFactorsLoadedEventArgs> FactorsLoaded;

        public DefaultCurrenciesFactorsUpdateStrategy(ICurrenciesFactorsLoader factorsLoader
            , TimerScheduleCallback scheduleCallback)
        {
            if (factorsLoader == null)
                throw new ArgumentNullException("factorsLoader");
            if (scheduleCallback == null)
                throw new ArgumentNullException("scheduleCallback");

            _factorsLoader = factorsLoader;
            _scheduleCallback = scheduleCallback;
        }

        ~DefaultCurrenciesFactorsUpdateStrategy()
        {
            Dispose(false);
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

        public void ForceUpdate()
        {
            _timer.Enabled = false;
            UpdateFactorsAsync();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (disposing)
            {
                try { _timer.Stop(); }
                catch { } 
                _timer.Dispose();

                GC.SuppressFinalize(this);
            }

            try
            { _factorsLoader.CancelAsync(); }
            catch
            { }
            _factorsLoader.Dispose();
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
            _timer.AutoReset = true;
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
            var args = new CurrencyFactorsLoadedEventArgs(_scheduleCallback(true));

            try
            {
                var factorsLoaded = FactorsLoaded;
                if (factorsLoaded != null)
                    factorsLoaded(this, args);
            }
            finally
            {
                StartTimer(args.ReloadFactorsTimeout);
            }
        }

        private void HandleUpdateFactorsError(Exception ex)
        {
            var args = new CurrencyFactorsLoadingErrorEventArgs(ex, _scheduleCallback(false));

            try
            {
                var factorsLoadingError = FactorsLoadingError;
                if (factorsLoadingError != null)
                    factorsLoadingError(this, args);
            }
            finally
            {
                StartTimer(args.ReloadFactorsTimeout);
            }
        }

        private void StartTimer(int timeout)
        {
            const int minTimerTimeout = 100;

            _timer.Interval = Math.Max(timeout, minTimerTimeout);
            _timer.Start();
        }

        private void CheckDisposedFlag()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("DefaultCurrenciesFactorsUpdateStrategy");
        }
    }
}