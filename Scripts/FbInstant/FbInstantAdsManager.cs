#if UNIT_FBINSTANT
namespace UniT.Advertisements
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Cysharp.Threading.Tasks;
    using UniT.FbInstant;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public sealed class FbInstantAdsManager : IInitializable, IAdsManager
    {
        #region Constructor

        private readonly IFbInstantAdvertisementConfig config;
        private readonly ILogger                       logger;

        [Preserve]
        public FbInstantAdsManager(IFbInstantAdvertisementConfig config, ILogger logger = null)
        {
            this.config = config;
            this.logger = logger ?? ILogger.Default(this.GetType().Name);
            this.logger.Debug("Constructed");
        }

        void IInitializable.Initialize()
        {
            this.LoadInterstitialAd();
            this.LoadRewardedAd();
            this.logger.Debug("Initialized");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this.logger.Config;

        public void ShowBannerAd() => this.InvokeUntilSuccess(this.config.BannerAdIds, FbInstant.Advertisements.ShowBannerAd);

        public void HideBannerAd() => this.InvokeOnce(FbInstant.Advertisements.HideBannerAd);

        public bool IsInterstitialAdReady() => this.config.InterstitialAdIds.Any(FbInstant.Advertisements.IsInterstitialAdReady);

        public void ShowInterstitialAd(Action onComplete = null) => this.ShowAd(
            adIds: this.config.InterstitialAdIds,
            isAdReady: FbInstant.Advertisements.IsInterstitialAdReady,
            showAction: FbInstant.Advertisements.ShowInterstitialAd,
            reloadAction: this.LoadInterstitialAd,
            onSuccess: null,
            onComplete: onComplete
        );

        public bool IsRewardedAdReady() => this.config.RewardedAdIds.Any(FbInstant.Advertisements.IsRewardedAdReady);

        public void ShowRewardedAd(Action onSuccess, Action onComplete = null) => this.ShowAd(
            adIds: this.config.RewardedAdIds,
            isAdReady: FbInstant.Advertisements.IsRewardedAdReady,
            showAction: FbInstant.Advertisements.ShowRewardedAd,
            reloadAction: this.LoadRewardedAd,
            onSuccess: onSuccess,
            onComplete: onComplete
        );

        #endregion

        #region Private

        private void LoadInterstitialAd() => this.InvokeUntilSuccess(this.config.InterstitialAdIds, FbInstant.Advertisements.LoadInterstitialAd);

        private void LoadRewardedAd() => this.InvokeUntilSuccess(this.config.RewardedAdIds, FbInstant.Advertisements.LoadRewardedAd);

        private static readonly int[] RetryIntervals = { 4, 8, 16, 32, 64 };

        private void InvokeUntilSuccess(string[] adIds, Func<string, UniTask<Result>> action, [CallerMemberName] string caller = null)
        {
            UniTask.Void(async () =>
            {
                for (var index = 0;; ++index)
                {
                    var adId   = adIds[Mathf.Min(index, adIds.Length - 1)];
                    var result = await action(adId);
                    if (result.IsSuccess) break;
                    this.logger.Error($"{caller} error {index + 1} time(s): {result.Error}");
                    var retryInterval = RetryIntervals[Mathf.Min(index, RetryIntervals.Length - 1)];
                    await UniTask.WaitForSeconds(retryInterval);
                }
                this.logger.Debug($"{caller} success");
            });
        }

        private void ShowAd(string[] adIds, Func<string, bool> isAdReady, Func<string, UniTask<Result>> showAction, Action reloadAction, Action onSuccess, Action onComplete, [CallerMemberName] string caller = null)
        {
            var adId = adIds.FirstOrDefault(isAdReady);
            if (adId is null)
            {
                this.logger.Error($"{caller} error: No ad ready");
                onComplete?.Invoke();
                return;
            }
            this.InvokeOnce(() => showAction(adId), onSuccess, reloadAction + onComplete, caller);
        }

        private void InvokeOnce(Func<UniTask<Result>> action, Action onSuccess = null, Action onComplete = null, [CallerMemberName] string caller = null)
        {
            action().ContinueWith(result =>
            {
                if (result.IsSuccess)
                {
                    this.logger.Debug($"{caller} success");
                    onSuccess?.Invoke();
                }
                else
                {
                    this.logger.Error($"{caller} error: {result.Error}");
                }
                onComplete?.Invoke();
            }).Forget();
        }

        #endregion
    }
}
#endif