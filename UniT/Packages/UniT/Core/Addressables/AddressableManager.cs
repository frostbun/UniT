namespace UniT.Core.Addressables
{
    using System.Collections.Generic;
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

        public void Release(string key)
        {
            this.handleCache.Remove(key, out var handle);
            Addressables.Release(handle);
            this.logger.Log($"Released addressable {key}");
        }

        public UniTask<T> Load<T>(string key, bool cache = false)
        {
            return this.handleCache.GetOrAdd(key, () => Addressables.LoadAssetAsync<T>(key))
                       .Convert<T>()
                       .ToUniTask()
                       .ContinueWith(asset =>
                       {
                           this.logger.Log($"Loaded addressable {key}");
                           if (!cache) this.Release(key);
                           return asset;
                       });
        }

        public UniTask<SceneInstance> LoadScene(string key, bool activateOnLoad = true, int priority = 100)
        {
            var handle = Addressables.LoadSceneAsync(key, activateOnLoad: activateOnLoad, priority: priority);
            return handle.ToUniTask()
                         .ContinueWith(scene =>
                         {
                             if (activateOnLoad)
                             {
                                 Addressables.Release(handle);
                                 this.logger.Log($"Scene {key} loaded");
                             }
                             else
                             {
                                 this.logger.Warning($"Scene {key} loaded & must be released manually");
                             }

                             return scene;
                         });
        }
    }
}