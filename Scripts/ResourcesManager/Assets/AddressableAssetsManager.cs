namespace UniT.ResourcesManager
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;

    public sealed class AddressableAssetsManager : AssetsManager
    {
        [Preserve]
        public AddressableAssetsManager(ILogger logger = null) : base(logger)
        {
        }

        protected override Object Load(string key)
        {
            return Addressables.LoadAssetAsync<Object>(key).WaitForCompletion();
        }

        protected override UniTask<Object> LoadAsync(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return Addressables.LoadAssetAsync<Object>(key).ToUniTask(progress: progress, cancellationToken: cancellationToken);
        }

        protected override void Unload(Object @object)
        {
            Addressables.Release(@object);
        }
    }
}