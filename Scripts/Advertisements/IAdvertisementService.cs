﻿namespace UniT.Advertisements
{
    using System;
    using UniT.Logging;

    public interface IAdvertisementService
    {
        public LogConfig LogConfig { get; }

        public void ShowBannerAd();

        public void HideBannedAd();

        public bool IsInterstitialAdReady();

        public void LoadInterstitialAd();

        public void ShowInterstitialAd(Action onComplete = null);

        public bool IsRewardedAdReady();

        public void LoadRewardedAd();

        public void ShowRewardedAd(Action onSuccess, Action onComplete = null);
    }
}