namespace UniT.Advertisements
{
    using System;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public sealed class DummyAdsManager : IAdsManager
    {
        #region Constructor

        private readonly ILogger _logger;

        [Preserve]
        public DummyAdsManager(ILogger logger = null)
        {
            this._logger = logger ?? ILogger.Default(this.GetType().Name);
            this._logger.Debug("Constructed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this._logger.Config;

        public void ShowBannerAd()
        {
            this._logger.Info("ShowBannerAd");
        }

        public void HideBannerAd()
        {
            this._logger.Info("HideBannedAd");
        }

        public bool IsInterstitialAdReady()
        {
            this._logger.Debug("IsInterstitialAdReady");
            return true;
        }

        public void ShowInterstitialAd(Action onComplete = null)
        {
            this._logger.Info("ShowInterstitialAd");
            onComplete?.Invoke();
        }

        public bool IsRewardedAdReady()
        {
            this._logger.Debug("IsRewardedAdReady");
            return true;
        }

        public void ShowRewardedAd(Action onSuccess, Action onComplete = null)
        {
            this._logger.Info("ShowRewardedAd");
            onSuccess?.Invoke();
            onComplete?.Invoke();
        }

        #endregion
    }
}