#if UNIT_ADDRESSABLES
#nullable enable
namespace UniT.ResourcesManager
{
    using System;
    using UniT.Logging;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Scripting;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class AddressableAssetsManager : AssetsManager
    {
        [Preserve]
        public AddressableAssetsManager(ILoggerManager loggerManager) : base(loggerManager)
        {
        }

        protected override Object? Load<T>(string key)
        {
            return Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
        }

        protected override void Unload(Object asset)
        {
            Addressables.Release(asset);
        }

        #if UNIT_UNITASK
        protected override UniTask<Object?> LoadAsync<T>(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return Addressables.LoadAssetAsync<T>(key)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(asset => (Object?)asset);
        }
        #else
        protected override IEnumerator LoadAsync<T>(string key, Action<Object?> callback, IProgress<float>? progress)
        {
            var operation = Addressables.LoadAssetAsync<T>(key);
            while (!operation.IsDone)
            {
                progress?.Report(operation.PercentComplete);
                yield return null;
            }
            callback(operation.Result);
        }
        #endif
    }
}
#endif