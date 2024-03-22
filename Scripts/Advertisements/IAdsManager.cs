namespace UniT.Advertisements
{
    using System;
    using UniT.Initializables;

    public interface IAdsManager : IInitializable
    {
        public void ShowBannerAd();

        public void HideBannerAd();

        public bool IsInterstitialAdReady();

        public void ShowInterstitialAd(Action onComplete = null);

        public bool IsRewardedAdReady();

        public void ShowRewardedAd(Action onSuccess, Action onComplete = null);
    }
}