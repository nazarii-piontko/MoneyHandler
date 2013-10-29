using System;
using System.Threading;
using MoneyHandler.CurrenciesFactorLoaders;
using MoneyHandler.CurrenciesFactorProviders;
using MoneyHandler.CurrenciesFactorsUpdateStrategies;

namespace MoneyHandler
{
    public class MoneyHandlerSettings
    {
        #region Static

        private static readonly object StaticInstanceLocker = new object();
        private static MoneyHandlerSettings _staticInstance;

        /// <summary>
        /// Initialize money handler settings singelton stuff.
        /// 
        /// Default configuration is:
        /// new MoneyHandlerSettings(new DefaultCurrenciesFactorsUpdateStrategy(new YahooCurrenciesFactorsLoader(), isPreviousLoadSuccessful => isPreviousLoadSuccessful ? (6*60*60*1000) : 60*1000), Currency.USD);
        /// </summary>
        public static void Init(MoneyHandlerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            lock (StaticInstanceLocker)
            {
                if (_staticInstance == settings)
                    return;

                if (_staticInstance != null)
                    _staticInstance.FactorsUpdateStrategy.Dispose();

                _staticInstance = settings;
            }
        }

        public static void InitDefault()
        {
            Init(CreateDefaultSettings());
        }

        public static MoneyHandlerSettings Instance
        {
            get
            {
                if (_staticInstance == null)
                {
                    lock (StaticInstanceLocker)
                    {
                        if (Interlocked.CompareExchange(ref _staticInstance, null, null) == null)
                        {
                            var settings = CreateDefaultSettings();

                            _staticInstance = settings;
                        }
                    }
                }

                return _staticInstance;
            }
        }

        private static MoneyHandlerSettings CreateDefaultSettings()
        {
            return
                new MoneyHandlerSettings(
                    new DefaultCurrenciesFactorsUpdateStrategy(new YahooCurrenciesFactorsLoader(),
                                                               isPreviousLoadSuccessful =>
                                                               isPreviousLoadSuccessful ? (6*60*60*1000) : 60*1000),
                    Currency.USD);
        }

        #endregion

        private readonly ICurrenciesFactorProvider _factorProvider;
        private readonly ICurrenciesFactorsUpdateStrategy _factorsUpdateStrategy;
        private readonly Currency _defaultCurrency;

        public ICurrenciesFactorsUpdateStrategy FactorsUpdateStrategy
        {
            get { return _factorsUpdateStrategy; }
        }

        public ICurrenciesFactorProvider FactorProvider
        {
            get { return _factorProvider; }
        }

        public Currency DefaultCurrency
        {
            get { return _defaultCurrency; }
        }

        public MoneyHandlerSettings(ICurrenciesFactorsUpdateStrategy factorsUpdateStrategy, Currency defaultCurrency)
        {
            _factorsUpdateStrategy = factorsUpdateStrategy;
            _defaultCurrency = defaultCurrency;
            _factorProvider = factorsUpdateStrategy.CreateAndInitProvider();
        }
    }
}