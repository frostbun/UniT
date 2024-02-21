namespace UniT.ResourcesManager
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
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
        public ResourceAssetsManager(ILogger.IFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Object Load(string key)
        {
            return Resources.Load(key);
        }

        protected override void Unload(Object obj)
        {
            Resources.UnloadAsset(obj);
        }

        #if UNIT_UNITASK
        protected override UniTask<Object> LoadAsync(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return Resources.LoadAsync(key).ToUniTask(progress: progress, cancellationToken: cancellationToken);
        }
        #else
        protected override IEnumerator LoadAsync(string key, Action<Object> callback, IProgress<float> progress)
        {
            var request = Resources.LoadAsync(key);
            while (!request.isDone)
            {
                progress?.Report(request.progress);
                yield return null;
            }
            callback(request.asset);
        }
        #endif
    }
}