#nullable enable
namespace UniT.ResourcesManager
{
    using System;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.Scripting;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class ResourceAssetsManager : AssetsManager
    {
        [Preserve]
        public ResourceAssetsManager(ILoggerManager loggerManager) : base(loggerManager)
        {
        }

        protected override Object? Load<T>(string key)
        {
            return Resources.Load<T>(key);
        }

        protected override void Unload(Object asset)
        {
            Resources.UnloadAsset(asset);
        }

        #if UNIT_UNITASK
        protected override UniTask<Object?> LoadAsync<T>(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return Resources.LoadAsync<T>(key)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken);
        }
        #else
        protected override IEnumerator LoadAsync<T>(string key, Action<Object?> callback, IProgress<float>? progress)
        {
            var operation = Resources.LoadAsync<T>(key);
            while (!operation.isDone)
            {
                progress?.Report(operation.progress);
                yield return null;
            }
            callback(operation.asset);
        }
        #endif
    }
}