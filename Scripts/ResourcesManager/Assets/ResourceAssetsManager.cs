﻿namespace UniT.ResourcesManager
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

    [Preserve]
    public sealed class ResourceAssetsManager : AssetsManager
    {
        public ResourceAssetsManager(ILogger.IFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Object Load<T>(string key)
        {
            return Resources.Load<T>(key);
        }

        protected override void Unload(Object obj)
        {
            Resources.UnloadAsset(obj);
        }

        #if UNIT_UNITASK
        protected override UniTask<Object> LoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return Resources.LoadAsync<T>(key)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken);
        }
        #else
        protected override IEnumerator LoadAsync<T>(string key, Action<Object> callback, IProgress<float> progress)
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