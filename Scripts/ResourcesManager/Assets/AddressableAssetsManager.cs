#if UNIT_ADDRESSABLES
namespace UniT.ResourcesManager
{
    using System;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
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
        public AddressableAssetsManager(ILogger.IFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Object Load<T>(string key)
        {
            return Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
        }

        protected override void Unload(Object obj)
        {
            Addressables.Release(obj);
        }

        #if UNIT_UNITASK
        protected override async UniTask<Object> LoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return await Addressables.LoadAssetAsync<T>(key).ToUniTask(progress: progress, cancellationToken: cancellationToken);
        }
        #else
        protected override IEnumerator LoadAsync<T>(string key, Action<Object> callback, IProgress<float> progress)
        {
            var request = Addressables.LoadAssetAsync<T>(key);
            while (!request.IsDone)
            {
                progress?.Report(request.PercentComplete);
                yield return null;
            }
            callback(request.Result);
        }
        #endif
    }
}
#endif