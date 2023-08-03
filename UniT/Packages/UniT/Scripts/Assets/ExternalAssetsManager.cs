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

    public class ExternalAssetsManager : IExternalAssetsManager
    {
        public LogConfig LogConfig => this._logger.Config;

        private readonly Dictionary<string, UnityWebRequestAsyncOperation> _loadedAssets;
        private readonly ILogger                                           _logger;

        [Preserve]
        public ExternalAssetsManager(ILogger logger = null)
        {
            this._loadedAssets = new();
            this._logger       = logger ?? ILogger.Default(this.GetType().Name);
        }

        public UniTask<Texture2D> DownloadTexture(string url, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return this._loadedAssets.GetOrAdd(url, () =>
                       {
                           var request = new UnityWebRequest(url);
                           request.downloadHandler = new DownloadHandlerTexture();
                           return request.SendWebRequest();
                       })
                       .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                       .ContinueWith(request => ((DownloadHandlerTexture)request.downloadHandler).texture);
        }

        public void Unload(string key)
        {
            if (!this._loadedAssets.Remove(key, out var request))
            {
                this._logger.Warning($"Trying to unload asset {key} that was not loaded");
                return;
            }
            request.webRequest.Dispose();
            this._logger.Debug($"Unloaded asset {key}");
        }
    }
}