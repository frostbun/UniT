namespace UniT.Advertisements
{
    using System;
    using UniT.Initializables;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public sealed class DummyAdsManager : IAdsManager
    {
        #region Constructor

        private readonly ILogger logger;

        [Preserve]
        public DummyAdsManager(ILoggerManager loggerManager)
        {
            this.logger = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

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