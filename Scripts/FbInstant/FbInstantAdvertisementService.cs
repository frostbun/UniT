#if UNIT_FBINSTANT
namespace UniT.Advertisements
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Cysharp.Threading.Tasks;
    using FbInstant.Advertisements;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public class FbInstantAdvertisementService : IInitializable, IAdvertisementService
    {
        #region Constructor

        private readonly IFbInstantAdvertisementConfig _config;
        private readonly FbInstantAdvertisement        _advertisement;
        private readonly ILogger                       _logger;

        [Preserve]
        public FbInstantAdvertisementService(IFbInstantAdvertisementConfig config, FbInstantAdvertisement advertisement, ILogger logger = null)
        {
            this._config        = config;
            this._advertisement = advertisement;
            this._logger        = logger ?? ILogger.Default(this.GetType().Name);
        }

        void IInitializable.Initialize()
        {
            this.ShowBannerAd();
            this.LoadInterstitialAd();
            this.LoadRewardedAd();
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this._logger.Config;

        public void ShowBannerAd() => this.Invoke(this._config.BannerAdIds, this._advertisement.ShowBannerAd);

        public void HideBannedAd() => this.InvokeOnce(this._advertisement.HideBannerAd);

        public bool IsInterstitialAdReady() => this._config.InterstitialAdIds.Any(this._advertisement.IsInterstitialAdReady);

        public void ShowInterstitialAd(Action onComplete = null) => this.InvokeOnce(this._config.InterstitialAdIds, this._advertisement.IsInterstitialAdReady, this._advertisement.ShowInterstitialAd, this.LoadInterstitialAd, onComplete);

        public bool IsRewardedAdReady() => this._config.RewardedAdIds.Any(this._advertisement.IsRewardedAdReady);

        public void ShowRewardedAd(Action onSuccess, Action onComplete = null) => this.InvokeOnce(this._config.RewardedAdIds, this._advertisement.IsRewardedAdReady, this._advertisement.ShowRewardedAd, this.LoadRewardedAd + onSuccess, onComplete);

        #endregion

        #region Private

        private void LoadInterstitialAd() => this.Invoke(this._config.InterstitialAdIds, this._advertisement.LoadInterstitialAd);

        private void LoadRewardedAd() => this.Invoke(this._config.RewardedAdIds, this._advertisement.LoadRewardedAd);

        private static readonly int[] RetryIntervals = { 4, 8, 16, 32, 64 };

        private void Invoke(string[] adIds, Func<string, UniTask<string>> action, [CallerMemberName] string caller = null)
        {
            UniTask.Void(async () =>
            {
                for (var index = 0;; ++index)
                {
                    var adId  = adIds[Math.Min(index, adIds.Length - 1)];
                    var error = await action(adId);
                    if (error is null) break;
                    this._logger.Error($"{caller} error {index + 1} time(s): {error}");
                    var retryInterval = RetryIntervals[Math.Min(index, RetryIntervals.Length - 1)];
                    await UniTask.WaitForSeconds(retryInterval);
                }
                this._logger.Debug($"{caller} success");
            });
        }

        private void InvokeOnce(string[] adIds, Func<string, bool> check, Func<string, UniTask<string>> action, Action onSuccess = null, Action onComplete = null, [CallerMemberName] string caller = null)
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

        private void InvokeOnce(Func<UniTask<string>> action, Action onSuccess = null, Action onComplete = null, [CallerMemberName] string caller = null)
        {
            action().ContinueWith(error =>
            {
                if (error is null)
                {
                    this._logger.Debug($"{caller} success");
                    onSuccess?.Invoke();
                }
                else
                {
                    this._logger.Error($"{caller} error: {error}");
                }
            }).Finally(() => onComplete?.Invoke()).Forget();
        }

        #endregion
    }
}
#endif