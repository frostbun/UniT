namespace UniT.Assets
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

        private readonly Dictionary<string, UnityWebRequestAsyncOperation> loadedAssets;
        private readonly ILogger                                           logger;

        [Preserve]
        public ExternalAssetsManager(ILogger logger = null)
        {
            this.loadedAssets = new();
            this.logger       = logger ?? ILogger.Default(this.GetType().Name);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Finalizer

        ~ExternalAssetsManager()
        {
            this.Dispose();
            this.logger.Debug("Finalized");
        }

        public void Dispose()
        {
            this.loadedAssets.Keys.SafeForEach(this.Unload);
            this.logger.Debug("Disposed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this.logger.Config;

        public UniTask<Texture2D> DownloadTexture(string url, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return this.loadedAssets
                .GetOrAdd(url, () =>
                {
                    var request = new UnityWebRequest(url);
                    request.downloadHandler = new DownloadHandlerTexture();
                    return request.SendWebRequest();
                })
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(request =>
                {
                    var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    this.logger.Debug($"Downloaded texture from {url}");
                    return texture;
                });
        }

        public void Unload(string key)
        {
            if (!this.loadedAssets.Remove(key, out var request))
            {
                this.logger.Warning($"Trying to unload asset {key} that was not loaded");
                return;
            }
            request.webRequest.Dispose();
            this.logger.Debug($"Unloaded asset {key}");
        }

        #endregion
    }
}