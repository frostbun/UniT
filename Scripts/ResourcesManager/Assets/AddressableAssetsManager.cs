#if UNIT_ADDRESSABLES
namespace UniT.ResourcesManager
{
    using UnityEngine.AddressableAssets;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public sealed class AddressableAssetsManager : AssetsManager
    {
        [Preserve]
        public AddressableAssetsManager(ILogger.IFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Object Load(string key)
        {
            return Addressables.LoadAssetAsync<Object>(key).WaitForCompletion();
        }

        protected override void Unload(Object @object)
        {
            Addressables.Release(@object);
        }

        #if UNIT_UNITASK
        protected override UniTask<Object> LoadAsync(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return Addressables.LoadAssetAsync<Object>(key).ToUniTask(progress: progress, cancellationToken: cancellationToken);
        }
        #endif
    }
}
#endif