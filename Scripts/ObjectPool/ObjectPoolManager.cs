namespace UniT.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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

        private readonly Transform                                               poolsContainer = new GameObject(nameof(ObjectPoolManager)).DontDestroyOnLoad().transform;
        private readonly Dictionary<GameObject, ObjectPool>                      prefabToPool   = new();
        private readonly Dictionary<string, ObjectPool>                          keyToPool      = new();
        private readonly Dictionary<GameObject, ObjectPool>                      instanceToPool = new();
        private readonly Dictionary<GameObject, ReadOnlyCollection<IRecyclable>> recyclables    = new();

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
            this.Spawn(instance, pool);
            return instance;
        }

        GameObject IObjectPoolManager.Spawn(string key, Vector3 position, Quaternion rotation, Transform parent)
        {
            var pool     = this.GetPool(key);
            var instance = pool.Spawn(position, rotation, parent);
            this.Spawn(instance, pool);
            return instance;
        }

        void IObjectPoolManager.Recycle(GameObject instance)
        {
            if (!this.instanceToPool.Remove(instance, out var pool))
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
            if (!this.prefabToPool.Remove(prefab, out var pool))
            {
                this.logger.Warning($"Trying to unload {prefab.name} pool that was not instantiated");
                return;
            }
            this.Unload(pool);
        }

        void IObjectPoolManager.Unload(string key)
        {
            if (!this.keyToPool.Remove(key, out var pool))
            {
                this.logger.Warning($"Trying to unload {key} pool that was not instantiated");
                return;
            }
            this.Unload(pool);
            this.assetsManager.Unload(key);
        }

        #endregion

        #region Private

        private ObjectPool GetPool(GameObject prefab)
        {
            return this.prefabToPool.GetOrDefault(prefab)
                ?? throw new InvalidOperationException($"{prefab.name} pool was not instantiated");
        }

        private ObjectPool GetPool(string key)
        {
            return this.keyToPool.GetOrDefault(key)
                ?? throw new InvalidOperationException($"{key} pool was not instantiated");
        }

        private ObjectPool Load(GameObject prefab)
        {
            var pool = ObjectPool.Instantiate(prefab);
            pool.transform.SetParent(this.poolsContainer);
            this.logger.Debug($"Instantiated {pool.gameObject.name}");
            return pool;
        }

        private void Spawn(GameObject instance, ObjectPool pool)
        {
            this.instanceToPool.Add(instance, pool);
            this.recyclables.GetOrAdd(instance, () =>
            {
                var recyclables = instance.GetComponentsInChildren<IRecyclable>(true).AsReadOnly();
                recyclables.ForEach(recyclable =>
                {
                    recyclable.Manager = this;
                    recyclable.OnInstantiate();
                });
                this.logger.Debug($"Instantiated {instance.name}");
                return recyclables;
            }).ForEach(component => component.OnSpawn());
            this.logger.Debug($"Spawned {instance.name}");
        }

        private void Recycle(GameObject instance, ObjectPool pool)
        {
            pool.Recycle(instance);
            this.recyclables[instance].ForEach(recyclable => recyclable.OnRecycle());
            if (!instance)
            {
                this.recyclables.Remove(instance);
                this.logger.Warning($"Trying to recycle {instance.name} that was already destroyed");
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
            pool.GetComponentsInChildren<Transform>().ForEach(transform => this.recyclables.Remove(transform.gameObject));
            Object.Destroy(pool.gameObject);
            this.logger.Debug($"Destroyed {pool.gameObject.name}");
        }

        #endregion
    }
}