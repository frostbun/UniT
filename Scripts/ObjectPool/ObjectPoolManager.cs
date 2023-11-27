namespace UniT.ObjectPool
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

        private readonly Transform                             poolsContainer = new GameObject(nameof(ObjectPoolManager)).DontDestroyOnLoad().transform;
        private readonly Dictionary<GameObject, ObjectPool>    prefabToPool   = new();
        private readonly Dictionary<string, ObjectPool>        keyToPool      = new();
        private readonly Dictionary<GameObject, ObjectPool>    instanceToPool = new();
        private readonly Dictionary<GameObject, IRecyclable[]> recyclables    = new();

        [Preserve]
        public ObjectPoolManager(IAssetsManager assetsManager, ILogger logger)
        {
            this.assetsManager = assetsManager;
            this.logger        = logger;
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

        LogConfig IObjectPoolManager.LogConfig => this.logger.Config;

        void IObjectPoolManager.Load(GameObject prefab, int count)
        {
            var isLoaded = this.prefabToPool.TryAdd(prefab, () => this.Load(prefab, count));
            Debug.Log(isLoaded ? $"Using cached {prefab.name} Pool" : $"Loaded {prefab.name} Pool");
        }

        void IObjectPoolManager.Load(string key, int count)
        {
            var isLoaded = this.keyToPool.TryAdd(key, () => this.Load(this.assetsManager.Load<GameObject>(key), count));
            Debug.Log(isLoaded ? $"Using cached {key} Pool" : $"Loaded {key} Pool");
        }

        #if UNIT_UNITASK
        UniTask IObjectPoolManager.LoadAsync(string key, int count, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return this.keyToPool
                .TryAddAsync(key, () =>
                    this.assetsManager.LoadAsync<GameObject>(key, progress, cancellationToken)
                        .ContinueWith(prefab => this.Load(prefab, count))
                )
                .ContinueWith(isLoaded =>
                {
                    this.logger.Debug(isLoaded ? $"Loaded {key} Pool" : $"Using cached {key} Pool");
                });
        }
        #endif

        public GameObject Spawn(GameObject prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var pool     = this.GetPool(prefab);
            var instance = pool.Spawn(position, rotation, parent);
            this.Spawn_Internal(instance, pool);
            return instance;
        }

        public GameObject Spawn(string key, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var pool     = this.GetPool(key);
            var instance = pool.Spawn(position, rotation, parent);
            this.Spawn_Internal(instance, pool);
            return instance;
        }

        public void Recycle(GameObject instance)
        {
            if (!this.instanceToPool.Remove(instance, out var pool))
            {
                this.logger.Warning($"Trying to recycle {instance.name} that was not spawned from {this.GetType().Name}");
                return;
            }
            this.Recycle_Internal(instance, pool);
        }

        public void RecycleAll(GameObject prefab)
        {
            this.RecycleAll_Internal(this.GetPool(prefab));
        }

        public void RecycleAll(string key)
        {
            this.RecycleAll_Internal(this.GetPool(key));
        }

        public void Unload(GameObject prefab)
        {
            if (!this.prefabToPool.Remove(prefab, out var pool))
            {
                this.logger.Warning($"Trying to destroy pool for {prefab.name} that was not instantiated");
                return;
            }
            this.DestroyPool_Internal(pool);
        }

        public void Unload(string key)
        {
            if (!this.keyToPool.Remove(key, out var pool))
            {
                this.logger.Warning($"Trying to destroy pool for {key} that was not instantiated");
                return;
            }
            this.DestroyPool_Internal(pool);
            this.assetsManager.Unload(key);
        }

        #endregion

        #region Private

        private ObjectPool GetPool(GameObject prefab)
        {
            return this.prefabToPool.GetOrDefault(prefab)
                ?? throw new InvalidOperationException($"Pool for {prefab.name} was not instantiated");
        }

        private ObjectPool GetPool(string key)
        {
            return this.keyToPool.GetOrDefault(key)
                ?? throw new InvalidOperationException($"Pool for {key} was not instantiated");
        }

        private ObjectPool Load(GameObject prefab, int count)
        {
            var pool = ObjectPool.Instantiate(prefab, count);
            pool.transform.SetParent(this.poolsContainer);
            this.logger.Debug($"Instantiated {pool.gameObject.name}");
            return pool;
        }

        private void DestroyPool_Internal(ObjectPool pool)
        {
            this.RecycleAll_Internal(pool);
            pool.GetComponentsInChildren<Transform>().ForEach(transform => this.recyclables.Remove(transform.gameObject));
            Object.Destroy(pool.gameObject);
            this.logger.Debug($"Destroyed {pool.gameObject.name}");
        }

        private void Spawn_Internal(GameObject instance, ObjectPool pool)
        {
            this.instanceToPool.Add(instance, pool);
            this.recyclables.GetOrAdd(instance, () =>
            {
                var recyclables = instance.GetComponentsInChildren<IRecyclable>(true);
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

        private void Recycle_Internal(GameObject instance, ObjectPool pool)
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

        private void RecycleAll_Internal(ObjectPool pool)
        {
            this.instanceToPool.RemoveAll((instance, otherPool) =>
            {
                if (otherPool != pool) return false;
                this.Recycle_Internal(instance, pool);
                return true;
            });
            this.logger.Debug($"Recycled all {pool.gameObject.name}");
        }

        #endregion
    }
}