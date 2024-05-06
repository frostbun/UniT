namespace UniT.ResourcesManager
{
    using System;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IExternalAssetsManager : IDisposable
    {
        #if UNIT_UNITASK
        public UniTask<Texture2D> DownloadTextureAsync(string url, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator DownloadTextureAsync(string url, Action<Texture2D> callback, IProgress<float> progress = null);
        #endif

        public void Unload(string key);
    }
}