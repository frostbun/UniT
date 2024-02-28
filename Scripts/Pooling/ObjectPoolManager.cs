namespace UniT.Pooling
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.ResourcesManager;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public sealed class ObjectPoolManager : IObjectPoolManager
    {
        #region Constructor

        private readonly IAssetsManager assetsManager;
        private readonly ILogger        logger;

        private readonly Transform                          poolsContainer = new GameObject(nameof(ObjectPoolManager)).DontDestroyOnLoad().transform;
        private readonly Dictionary<GameObject, ObjectPool> prefabToPool   = new Dictionary<GameObject, ObjectPool>();
        private readonly Dictionary<string, ObjectPool>     keyToPool      = new Dictionary<string, ObjectPool>();
        private readonly Dictionary<GameObject, ObjectPool> instanceToPool = new Dictionary<GameObject, ObjectPool>();

        [Preserve]
        public ObjectPoolManager(IAssetsManager assetsManager, ILogger.IFactory loggerFactory)
        {
            this.assetsManager = assetsManager;
            this.logger        = loggerFactory.Create(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

        LogConfig IHasLogger.LogConfig => this.logger.Config;

        void IObjectPoolManager.Load(GameObject prefab, int count)
        {
            var isLoaded = this.prefabToPool.TryAdd(prefab, () => this.Load(prefab));
            this.logger.Debug(isLoaded ? $"Loaded {prefab.name} pool" : $"Using cached {prefab.name} pool");
            this.prefabToPool[prefab].Load(count);
        }

        void IObjectPoolManager.Load(string key, int count)
        {
            var isLoaded = this.keyToPool.TryAdd(key, () => this.Load(this.assetsManager.Load<GameObject>(key)));
            this.logger.Debug(isLoaded ? $"Loaded {key} pool" : $"Using cached {key} pool");
            this.keyToPool[key].Load(count);
        }

        #if UNIT_UNITASK
        async UniTask IObjectPoolManager.LoadAsync(string key, int count, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var isLoaded = await this.keyToPool.TryAddAsync(key, async () => this.Load(await this.assetsManager.LoadAsync<GameObject>(key, progress, cancellationToken)));
            this.logger.Debug(isLoaded ? $"Loaded {key} pool" : $"Using cached {key} pool");
            this.keyToPool[key].Load(count);
        }
        #endif

        GameObject IObjectPoolManager.Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var pool     = this.GetPool(prefab);
            var instance = pool.Spawn(position, rotation, parent);
            this.instanceToPool.Add(instance, pool);
            this.logger.Debug($"Spawned {prefab.name}");
            return instance;
        }

        GameObject IObjectPoolManager.Spawn(string key, Vector3 position, Quaternion rotation, Transform parent)
        {
            var pool     = this.GetPool(key);
            var instance = pool.Spawn(position, rotation, parent);
            this.instanceToPool.Add(instance, pool);
            this.logger.Debug($"Spawned {key}");
            return instance;
        }

        void IObjectPoolManager.Recycle(GameObject instance)
        {
            if (!this.instanceToPool.TryRemove(instance, out var pool))
            {
                this.logger.Warning($"Trying to recycle {instance.name} that was not spawned from {this.GetType().Name}");
                return;
            }
            this.Recycle(instance, pool);
        }

        void IObjectPoolManager.RecycleAll(GameObject prefab)
        {
            this.RecycleAll(this.GetPool(prefab));
        }

        void IObjectPoolManager.RecycleAll(string key)
        {
            this.RecycleAll(this.GetPool(key));
        }

        void IObjectPoolManager.Unload(GameObject prefab)
        {
            if (!this.prefabToPool.TryRemove(prefab, out var pool))
            {
                this.logger.Warning($"Trying to unload {prefab.name} pool that is not loaded");
                return;
            }
            this.Unload(pool);
        }

        void IObjectPoolManager.Unload(string key)
        {
            if (!this.keyToPool.TryRemove(key, out var pool))
            {
                this.logger.Warning($"Trying to unload {key} pool that is not loaded");
                return;
            }
            this.Unload(pool);
            this.assetsManager.Unload(key);
        }

        #endregion

        #region Private

        private ObjectPool GetPool(GameObject prefab)
        {
            var isLoaded = this.prefabToPool.TryAdd(prefab, () => this.Load(prefab));
            if (isLoaded) this.logger.Warning($"Auto loading {prefab.name} pool. Consider preloading it with `Load` method.");
            return this.prefabToPool[prefab];
        }

        private ObjectPool GetPool(string key)
        {
            var isLoaded = this.keyToPool.TryAdd(key, () => this.Load(this.assetsManager.Load<GameObject>(key)));
            if (isLoaded) this.logger.Warning($"Auto loading {key} pool. Consider preloading it with `Load` method.");
            return this.keyToPool[key];
        }

        private ObjectPool Load(GameObject prefab)
        {
            var pool = ObjectPool.Instantiate(prefab);
            pool.transform.SetParent(this.poolsContainer);
            this.logger.Debug($"Instantiated {pool.gameObject.name}");
            return pool;
        }

        private void Recycle(GameObject instance, ObjectPool pool)
        {
            pool.Recycle(instance);
            if (!instance)
            {
                this.logger.Warning($"Trying to recycle {instance.name} that is destroyed");
                return;
            }
            this.logger.Debug($"Recycled {instance.name}");
        }

        private void RecycleAll(ObjectPool pool)
        {
            this.instanceToPool.RemoveAll((instance, otherPool) =>
            {
                if (otherPool != pool) return false;
                this.Recycle(instance, pool);
                return true;
            });
            this.logger.Debug($"Recycled all {pool.gameObject.name}");
        }

        private void Unload(ObjectPool pool)
        {
            this.RecycleAll(pool);
            Object.Destroy(pool.gameObject);
            this.logger.Debug($"Destroyed {pool.gameObject.name}");
        }

        #endregion
    }
}