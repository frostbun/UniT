namespace UniT.Core.Addressables
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Core.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
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
    }
}