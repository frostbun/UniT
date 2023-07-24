namespace UniT.Assets
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public interface IExternalAssetsManager
    {
        public ILogger Logger { get; }

        public UniTask<Texture2D> DownloadTexture(string url, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public void Unload(string key);
    }
}