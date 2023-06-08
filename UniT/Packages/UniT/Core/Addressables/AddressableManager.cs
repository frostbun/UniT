namespace UniT.Core.Addressables
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Core.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using ILogger = UniT.Core.Logging.ILogger;

    public class AddressableManager : IAddressableManager
    {
        private readonly ILogger                                  logger;
        private readonly Dictionary<string, AsyncOperationHandle> handleCache;

        public AddressableManager(ILogger logger)
        {
            this.logger      = logger;
            this.handleCache = new();
            this.logger.Log($"{this.GetType().Name} instantiated", Color.green);
        }

        public UniTask<T> Load<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return this.handleCache.GetOrDefault(key, () => Addressables.LoadAssetAsync<T>(key))
                       .Convert<T>()
                       .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                       .ContinueWith(asset =>
                       {
                           this.logger.Log($"Loaded & cached addressable {key}");
                           progress?.Report(1);
                           return asset;
                       });
        }

        public UniTask<T> LoadOnce<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return Addressables.LoadAssetAsync<T>(key)
                               .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                               .ContinueWith(asset =>
                               {
                                   this.logger.Log($"Loaded addressable {key}");
                                   Addressables.Release(asset);
                                   progress?.Report(1);
                                   return asset;
                               });
        }

        public UniTask<SceneInstance> LoadScene(string key, bool activateOnLoad = true, int priority = 100, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return Addressables.LoadSceneAsync(key, activateOnLoad: activateOnLoad, priority: priority)
                               .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                               .ContinueWith(scene =>
                               {
                                   if (!activateOnLoad)
                                   {
                                       this.logger.Warning($"Scene {key} loaded & must be released manually");
                                       return scene;
                                   }

                                   this.logger.Log($"Scene {key} loaded");
                                   Addressables.Release(scene);
                                   progress?.Report(1);
                                   return scene;
                               });
        }

        public void Release(string key)
        {
            if (!this.handleCache.Remove(key, out var handle))
            {
                this.logger.Warning("Trying to release an addressable that was not cached");
                return;
            }

            Addressables.Release(handle);
            this.logger.Log($"Released addressable {key}");
        }
    }
}