namespace UniT.Advertisements
{
    using System;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public sealed class DummyAdsManager : IAdsManager
    {
        #region Constructor

        private readonly ILogger logger;

        [Preserve]
        public DummyAdsManager(ILogger logger = null)
        {
            this.logger = logger ?? ILogger.Default(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this.logger.Config;

        public void ShowBannerAd()
        {
            this.logger.Info("ShowBannerAd");
        }

        public void HideBannerAd()
        {
            this.logger.Info("HideBannedAd");
        }

        public bool IsInterstitialAdReady()
        {
            this.logger.Debug("IsInterstitialAdReady");
            return true;
        }

        public void ShowInterstitialAd(Action onComplete = null)
        {
            this.logger.Info("ShowInterstitialAd");
            onComplete?.Invoke();
        }

        public bool IsRewardedAdReady()
        {
            this.logger.Debug("IsRewardedAdReady");
            return true;
        }

        public void ShowRewardedAd(Action onSuccess, Action onComplete = null)
        {
            this.logger.Info("ShowRewardedAd");
            onSuccess?.Invoke();
            onComplete?.Invoke();
        }

        #endregion
    }
}