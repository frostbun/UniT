namespace UniT.ObjectPool
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Extensions;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class ObjectPoolManager : IObjectPoolManager
    {
        private readonly IAddressableManager                addressableManager;
        private readonly ILogger                            logger;
        private readonly Dictionary<GameObject, ObjectPool> prefabToPool;
        private readonly Dictionary<string, ObjectPool>     keyToPool;

        public ObjectPoolManager(IAddressableManager addressableManager, ILogger logger = null)
        {
            this.addressableManager = addressableManager;
            this.logger             = logger;
            this.prefabToPool       = Object.FindObjectsOfType<ObjectPool>().ToDictionary(pool => (GameObject)typeof(ObjectPool).GetField("prefab").GetValue(pool), pool => pool);
            this.keyToPool          = new();
            this.logger?.Info($"{nameof(ObjectPoolManager)} instantiated with {this.prefabToPool.Count} pre-created pool", Color.green);
        }

        private ObjectPool InstantiatePool_Internal(GameObject prefab, int initialCount = 1)
        {
            var pool = ObjectPool.Instantiate(prefab, initialCount);
            this.logger?.Debug($"Instantiated {pool.gameObject.name}");
            return pool;
        }

        public void InstantiatePool(GameObject prefab, int initialCount = 1)
        {
            this.prefabToPool.TryAdd(prefab, () => this.InstantiatePool_Internal(prefab, initialCount));
        }

        public void InstantiatePool<T>(T component, int initialCount = 1) where T : Component
        {
            this.InstantiatePool(component.gameObject, initialCount);
        }

        public UniTask InstantiatePool(string key, int initialCount = 1)
        {
            return this.keyToPool.TryAdd(key, () => this.addressableManager.Load<GameObject>(key).ContinueWith(prefab => this.InstantiatePool_Internal(prefab, initialCount)));
        }

        public UniTask InstantiatePool<T>(int initialCount = 1) where T : Component
        {
            return this.InstantiatePool(typeof(T).GetKeyAttribute(), initialCount);
        }

        public ObjectPool GetPool(GameObject prefab)
        {
            this.InstantiatePool(prefab);
            return this.prefabToPool[prefab];
        }

        public ObjectPool GetPool<T>(T component) where T : Component
        {
            return this.GetPool(component.gameObject);
        }

        public UniTask<ObjectPool> GetPool(string key)
        {
            return this.InstantiatePool(key).ContinueWith(() => this.keyToPool[key]);
        }

        public UniTask<ObjectPool> GetPool<T>() where T : Component
        {
            return this.GetPool(typeof(T).GetKeyAttribute());
        }

        private void DestroyPool_Internal(ObjectPool pool)
        {
            Object.Destroy(pool.gameObject);
            this.logger?.Debug($"Destroyed {pool.gameObject.name}");
        }

        public void DestroyPool(GameObject prefab)
        {
            if (!this.prefabToPool.Remove(prefab, out var pool))
            {
                this.logger?.Warning($"Trying to destroy pool for prefab {prefab.name} that was not instantiated");
                return;
            }

            this.DestroyPool_Internal(pool);
        }

        public void DestroyPool<T>(T component) where T : Component
        {
            this.DestroyPool(component.gameObject);
        }

        public void DestroyPool(string key)
        {
            if (!this.keyToPool.Remove(key, out var pool))
            {
                this.logger?.Warning($"Trying to destroy pool for key {key} that was not instantiated");
                return;
            }

            this.DestroyPool_Internal(pool);
        }

        public void DestroyPool<T>() where T : Component
        {
            this.DestroyPool(typeof(T).GetKeyAttribute());
        }
    }
}