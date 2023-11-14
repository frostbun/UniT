namespace UniT.Advertisements
{
    using System;
    using UniT.Logging;

    public interface IAdsManager
    {
        public LogConfig LogConfig { get; }

        public void ShowBannerAd();

        public void HideBannerAd();

        public bool IsInterstitialAdReady();

        public void ShowInterstitialAd(Action onComplete = null);

        public bool IsRewardedAdReady();

        public void ShowRewardedAd(Action onSuccess, Action onComplete = null);
    }
}