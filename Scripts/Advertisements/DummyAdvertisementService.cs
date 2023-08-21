namespace UniT.Advertisements
{
    using System;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public class DummyAdvertisementService : IAdvertisementService
    {
        #region Constructor

        private readonly ILogger _logger;

        [Preserve]
        public DummyAdvertisementService(ILogger logger = null)
        {
            this._logger = logger ?? ILogger.Default(this.GetType().Name);
            this._logger.Debug("Constructed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this._logger.Config;

        public void ShowBannerAd()
        {
            this._logger.Debug("ShowBannerAd");
        }

        public void HideBannedAd()
        {
            this._logger.Debug("HideBannedAd");
        }

        public bool IsInterstitialAdReady()
        {
            this._logger.Debug("IsInterstitialAdReady");
            return true;
        }

        public void ShowInterstitialAd(Action onComplete = null)
        {
            this._logger.Debug("ShowInterstitialAd");
            onComplete?.Invoke();
        }

        public bool IsRewardedAdReady()
        {
            this._logger.Debug("IsRewardedAdReady");
            return true;
        }

        public void ShowRewardedAd(Action onSuccess, Action onComplete = null)
        {
            this._logger.Debug("ShowRewardedAd");
            onSuccess?.Invoke();
            onComplete?.Invoke();
        }

        #endregion
    }
}