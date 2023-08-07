namespace UniT.Advertisements
{
    using System;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public class DummyAdvertisementService : IAdvertisementService
    {
        public LogConfig LogConfig => this._logger.Config;

        private readonly ILogger _logger;

        [Preserve]
        public DummyAdvertisementService(ILogger logger = null)
        {
            this._logger = logger ?? ILogger.Default(this.GetType().Name);
        }

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

        public void LoadInterstitialAd()
        {
            this._logger.Debug("LoadInterstitialAd");
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

        public void LoadRewardedAd()
        {
            this._logger.Debug("LoadRewardedAd");
        }

        public void ShowRewardedAd(Action onSuccess, Action onComplete = null)
        {
            this._logger.Debug("ShowRewardedAd");
            onSuccess?.Invoke();
            onComplete?.Invoke();
        }
    }
}