namespace UniT.Advertisements
{
    using System;
    using UniT.Initializables;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public sealed class DummyAdsManager : IAdsManager, IHasLogger
    {
        #region Constructor

        private readonly ILogger logger;

        [Preserve]
        public DummyAdsManager(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.Create(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

        LogConfig IHasLogger.LogConfig => this.logger.Config;

        void IInitializable.Initialize()
        {
            this.logger.Debug("Initialized");
        }

        void IAdsManager.ShowBannerAd()
        {
            this.logger.Info("ShowBannerAd");
        }

        void IAdsManager.HideBannerAd()
        {
            this.logger.Info("HideBannedAd");
        }

        bool IAdsManager.IsInterstitialAdReady()
        {
            this.logger.Debug("IsInterstitialAdReady");
            return true;
        }

        void IAdsManager.ShowInterstitialAd(Action onComplete)
        {
            this.logger.Info("ShowInterstitialAd");
            onComplete?.Invoke();
        }

        bool IAdsManager.IsRewardedAdReady()
        {
            this.logger.Debug("IsRewardedAdReady");
            return true;
        }

        void IAdsManager.ShowRewardedAd(Action onSuccess, Action onComplete)
        {
            this.logger.Info("ShowRewardedAd");
            onSuccess.Invoke();
            onComplete?.Invoke();
        }

        #endregion
    }
}