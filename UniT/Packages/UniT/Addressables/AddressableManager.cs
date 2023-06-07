namespace UniT.Addressables
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using ILogger = UniT.Logging.ILogger;

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

        public async UniTask<T> Load<T>(string key, bool cache = false)
        {
            var handle = this.handleCache.GetOrAdd(key, () => Addressables.LoadAssetAsync<T>(key));
            var asset  = await handle.Convert<T>();
            this.logger.Log($"Loaded addressable {key}");
            if (!cache) this.Release(key);
            return asset;
        }

        public UniTask<T> Load<T>(Type type, bool cache = false)
        {
            return this.Load<T>(type.GetKeyAttribute(), cache);
        }
    }
}