namespace UniT.Assets
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.Networking;
    using ILogger = UniT.Logging.ILogger;

    public class ExternalAssetsManager : IExternalAssetsManager
    {
        public ILogger Logger { get; }

        private readonly Dictionary<string, UnityWebRequestAsyncOperation> loadedAssets;

        public ExternalAssetsManager(ILogger logger = null)
        {
            this.loadedAssets = new();
            this.Logger       = logger ?? ILogger.Default(this.GetType().Name);
        }

        public UniTask<Texture2D> DownloadTexture(string url, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return this.loadedAssets.GetOrAdd(url, () =>
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
            if (!this.loadedAssets.Remove(key, out var request))
            {
                this.Logger.Warning($"Trying to unload asset {key} that was not loaded");
                return;
            }
            request.webRequest.Dispose();
            this.Logger.Debug($"Unloaded asset {key}");
        }
    }
}