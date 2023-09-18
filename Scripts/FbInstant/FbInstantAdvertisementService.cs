#if UNIT_FBINSTANT
namespace UniT.Advertisements
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Cysharp.Threading.Tasks;
    using FbInstant;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public sealed class FbInstantAdvertisementService : IInitializable, IAdvertisementService
    {
        #region Constructor

        private readonly IFbInstantAdvertisementConfig _config;
        private readonly ILogger                       _logger;

        [Preserve]
        public FbInstantAdvertisementService(IFbInstantAdvertisementConfig config, ILogger logger = null)
        {
            this._config = config;
            this._logger = logger ?? ILogger.Default(this.GetType().Name);
            this._logger.Debug("Constructed");
        }

        void IInitializable.Initialize()
        {
            this.LoadInterstitialAd();
            this.LoadRewardedAd();
            this._logger.Debug("Initialized");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this._logger.Config;

        public void ShowBannerAd() => this.InvokeUntilSuccess(this._config.BannerAdIds, FbInstant.Advertisements.ShowBannerAd);

        public void HideBannerAd() => this.InvokeOnce(FbInstant.Advertisements.HideBannerAd);

        public bool IsInterstitialAdReady() => this._config.InterstitialAdIds.Any(FbInstant.Advertisements.IsInterstitialAdReady);

        public void ShowInterstitialAd(Action onComplete = null) => this.InvokeOnce(this._config.InterstitialAdIds, FbInstant.Advertisements.IsInterstitialAdReady, FbInstant.Advertisements.ShowInterstitialAd, null, this.LoadInterstitialAd + onComplete);

        public bool IsRewardedAdReady() => this._config.RewardedAdIds.Any(FbInstant.Advertisements.IsRewardedAdReady);

        public void ShowRewardedAd(Action onSuccess, Action onComplete = null) => this.InvokeOnce(this._config.RewardedAdIds, FbInstant.Advertisements.IsRewardedAdReady, FbInstant.Advertisements.ShowRewardedAd, onSuccess, this.LoadRewardedAd + onComplete);

        #endregion

        #region Private

        private void LoadInterstitialAd() => this.InvokeUntilSuccess(this._config.InterstitialAdIds, FbInstant.Advertisements.LoadInterstitialAd);

        private void LoadRewardedAd() => this.InvokeUntilSuccess(this._config.RewardedAdIds, FbInstant.Advertisements.LoadRewardedAd);

        private static readonly int[] RetryIntervals = { 4, 8, 16, 32, 64 };

        private void InvokeUntilSuccess(string[] adIds, Func<string, UniTask<Result>> action, [CallerMemberName] string caller = null)
        {
            UniTask.Void(async () =>
            {
                for (var index = 0;; ++index)
                {
                    var adId   = adIds[Math.Min(index, adIds.Length - 1)];
                    var result = await action(adId);
                    if (result.IsSuccess) break;
                    this._logger.Error($"{caller} error {index + 1} time(s): {result.Error}");
                    var retryInterval = RetryIntervals[Math.Min(index, RetryIntervals.Length - 1)];
                    await UniTask.WaitForSeconds(retryInterval);
                }
                this._logger.Debug($"{caller} success");
            });
        }

        private void InvokeOnce(string[] adIds, Func<string, bool> check, Func<string, UniTask<Result>> action, Action onSuccess, Action onComplete, [CallerMemberName] string caller = null)
        {
            var adId = adIds.FirstOrDefault(check);
            if (adId is null)
            {
                this._logger.Error($"{caller} error: No ad ready");
                onComplete?.Invoke();
                return;
            }
            this.InvokeOnce(() => action(adId), onSuccess, onComplete, caller);
        }

        private void InvokeOnce(Func<UniTask<Result>> action, Action onSuccess = null, Action onComplete = null, [CallerMemberName] string caller = null)
        {
            action().ContinueWith(result =>
            {
                if (result.IsSuccess)
                {
                    this._logger.Debug($"{caller} success");
                    onSuccess?.Invoke();
                }
                else
                {
                    this._logger.Error($"{caller} error: {result.Error}");
                }
                onComplete?.Invoke();
            }).Forget();
        }

        #endregion
    }
}
#endif