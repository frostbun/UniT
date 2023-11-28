#if UNIT_UNITASK
namespace UniT.ResourcesManager
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UnityEngine;

    public interface IExternalAssetsManager : IHasLogger, IDisposable
    {
        public UniTask<Texture2D> DownloadTexture(string url, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public void Unload(string key);
    }
}
#endif