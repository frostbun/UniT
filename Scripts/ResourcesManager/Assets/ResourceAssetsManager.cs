namespace UniT.ResourcesManager
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;

    public sealed class ResourceAssetsManager : AssetsManager
    {
        [Preserve]
        public ResourceAssetsManager(ILogger logger = null) : base(logger)
        {
        }

        protected override Object Load(string key)
        {
            return Resources.Load(key);
        }

        protected override UniTask<Object> LoadAsync(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return Resources.LoadAsync(key).ToUniTask(progress: progress, cancellationToken: cancellationToken);
        }

        protected override void Unload(Object @object)
        {
            Resources.UnloadAsset(@object);
        }
    }
}