using System;
using System.Threading;
using MoneyHandler.CurrenciesFactorLoader;
using MoneyHandler.CurrenciesFactorProvider;
using MoneyHandler.CurrenciesFactorsUpdateStrategy;

namespace MoneyHandler
{
    public class MoneyHandlerSettings
    {
        #region Static

        private static MoneyHandlerSettings _staticInstance;

        public static void Init(MoneyHandlerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            if (_staticInstance == settings)
                return;

            if (_staticInstance != null)
                _staticInstance.FactorsUpdateStrategy.Dispose();

            _staticInstance = settings;
        }

        public static MoneyHandlerSettings Instance
        {
            get
            {
                if (_staticInstance == null)
                {
                    MoneyHandlerSettings settings = CreateDefaultSettings();

                    Interlocked.CompareExchange(ref _staticInstance, settings, null);
                }
                return _staticInstance;
            }
        }

        private static MoneyHandlerSettings CreateDefaultSettings()
        {
            return new MoneyHandlerSettings(new DefaultCurrenciesFactorsUpdateStrategy(new YahooCurrenciesFactorsLoader()), Currency.USD);
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

        public MoneyHandlerSettings(ICurrenciesFactorsUpdateStrategy factorsUpdateStrategy)
            : this(factorsUpdateStrategy, Currency.USD)
        { }

        public MoneyHandlerSettings(ICurrenciesFactorsUpdateStrategy factorsUpdateStrategy, Currency defaultCurrency)
        {
            _factorsUpdateStrategy = factorsUpdateStrategy;
            _defaultCurrency = defaultCurrency;
            _factorProvider = factorsUpdateStrategy.CreateAndInitProvider();
        }
    }
}