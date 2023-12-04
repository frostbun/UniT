#if UNIT_UNITASK
namespace UniT.ResourcesManager
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public sealed class ExternalAssetsManager : IExternalAssetsManager
    {
        #region Constructor

        private readonly ILogger logger;

        private readonly Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

        [Preserve]
        public ExternalAssetsManager(ILogger.IFactory loggerFactory)
        {
            this.logger = loggerFactory.Create(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

        LogConfig IHasLogger.LogConfig => this.logger.Config;

        UniTask<Texture2D> IExternalAssetsManager.DownloadTexture(string url, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return this.cache
                .TryAddAsync(url, () => this.DownloadTexture(url, progress, cancellationToken))
                .ContinueWith(isLoaded =>
                {
                    this.logger.Debug(isLoaded ? $"Downloaded texture from {url}" : $"Using cached texture from {url}");
                    return this.cache[url];
                });
        }

        void IExternalAssetsManager.Unload(string key)
        {
            if (this.cache.Remove(key))
            {
                this.logger.Debug($"Unloaded {key}");
            }
            else
            {
                this.logger.Warning($"Trying to unload {key} that was not loaded");
            }
        }

        #endregion

        #region Private

        private UniTask<Texture2D> DownloadTexture(string url, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return new UnityWebRequest(url) { downloadHandler = new DownloadHandlerTexture() }
                .SendWebRequest()
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(request =>
                {
                    using var r = request;
                    return ((DownloadHandlerTexture)r.downloadHandler).texture;
                });
        }

        #endregion

        #region Finalizer

        private void Dispose()
        {
            this.cache.Clear();
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
            this.logger.Debug("Disposed");
        }

        ~ExternalAssetsManager()
        {
            this.Dispose();
            this.logger.Debug("Finalized");
        }

        #endregion
    }
}
#endif