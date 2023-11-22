namespace UniT.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;

    public sealed class ObjectPoolManager : IObjectPoolManager
    {
        #region Constructor

        private readonly IAssetsManager                        assetsManager;
        private readonly Transform                             poolsContainer;
        private readonly Dictionary<GameObject, ObjectPool>    prefabToPool;
        private readonly Dictionary<string, ObjectPool>        keyToPool;
        private readonly Dictionary<GameObject, ObjectPool>    instanceToPool;
        private readonly Dictionary<GameObject, IRecyclable[]> recyclables;
        private readonly ILogger                               logger;

        [Preserve]
        public ObjectPoolManager(IAssetsManager assetsManager = null, ILogger logger = null)
        {
            this.assetsManager  = assetsManager ?? IAssetsManager.Default();
            this.poolsContainer = new GameObject(this.GetType().Name).DontDestroyOnLoad().transform;
            this.prefabToPool   = new();
            this.keyToPool      = new();
            this.instanceToPool = new();
            this.recyclables    = new();
            this.logger         = logger ?? ILogger.Default(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Finalizer

        ~ObjectPoolManager()
        {
            this.Dispose();
            this.logger.Debug("Finalized");
        }

        public void Dispose()
        {
            this.prefabToPool.Keys.SafeForEach(this.DestroyPool);
            this.keyToPool.Keys.SafeForEach(this.DestroyPool);
            Object.Destroy(this.poolsContainer.gameObject);
            this.logger.Debug("Disposed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this.logger.Config;

        public void InstantiatePool(GameObject prefab, int initialCount = 1)
        {
            if (this.prefabToPool.TryAdd(prefab, () => this.InstantiatePool_Internal(prefab, initialCount))) return;
            this.logger.Warning($"Pool for {prefab.name} already instantiated");
        }

        public void InstantiatePool<T>(T component, int initialCount = 1) where T : Component
        {
            this.InstantiatePool(component.gameObject, initialCount);
        }

        public UniTask InstantiatePool(string key, int initialCount = 1)
        {
            return this.keyToPool.TryAddAsync(key, () => this.assetsManager.Load<GameObject>(key).ContinueWith(prefab => this.InstantiatePool_Internal(prefab, initialCount)))
                .ContinueWith(isSuccess =>
                {
                    if (isSuccess) return;
                    this.logger.Warning($"Pool for {key} already instantiated");
                });
        }

        public UniTask InstantiatePool<T>(int initialCount = 1) where T : Component
        {
            return this.InstantiatePool(typeof(T).GetKey(), initialCount);
        }

        public bool IsPoolReady(GameObject prefab)
        {
            return this.prefabToPool.ContainsKey(prefab);
        }

        public bool IsPoolReady<T>(T component) where T : Component
        {
            return this.IsPoolReady(component.gameObject);
        }

        public bool IsPoolReady(string key)
        {
            return this.keyToPool.ContainsKey(key);
        }

        public bool IsPoolReady<T>() where T : Component
        {
            return this.IsPoolReady(typeof(T).GetKey());
        }

        public void DestroyPool(GameObject prefab)
        {
            if (!this.prefabToPool.Remove(prefab, out var pool))
            {
                this.logger.Warning($"Trying to destroy pool for {prefab.name} that was not instantiated");
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
                this.logger.Warning($"Trying to destroy pool for {key} that was not instantiated");
                return;
            }
            this.DestroyPool_Internal(pool);
            this.assetsManager.Unload(key);
        }

        public void DestroyPool<T>() where T : Component
        {
            this.DestroyPool(typeof(T).GetKey());
        }

        public GameObject Spawn(GameObject prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var pool     = this.GetPool(prefab);
            var instance = pool.Spawn(position, rotation, parent);
            this.Spawn_Internal(instance, pool);
            return instance;
        }

        public T Spawn<T>(T component, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where T : Component
        {
            var pool     = this.GetPool(component.gameObject);
            var instance = pool.Spawn<T>(position, rotation, parent);
            this.Spawn_Internal(instance.gameObject, pool);
            return instance;
        }

        public GameObject Spawn(string key, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var pool     = this.GetPool(key);
            var instance = pool.Spawn(position, rotation, parent);
            this.Spawn_Internal(instance, pool);
            return instance;
        }

        public T Spawn<T>(string key = null, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where T : Component
        {
            var pool     = this.GetPool(key ?? typeof(T).GetKey());
            var instance = pool.Spawn<T>(position, rotation, parent);
            this.Spawn_Internal(instance.gameObject, pool);
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

        public void Recycle<T>(T component) where T : Component
        {
            this.Recycle(component.gameObject);
        }

        public void RecycleAll(GameObject prefab)
        {
            this.RecycleAll_Internal(this.GetPool(prefab));
        }

        public void RecycleAll<T>(T component) where T : Component
        {
            this.RecycleAll(component.gameObject);
        }

        public void RecycleAll(string key)
        {
            this.RecycleAll_Internal(this.GetPool(key));
        }

        public void RecycleAll<T>() where T : Component
        {
            this.RecycleAll(typeof(T).GetKey());
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

        private ObjectPool InstantiatePool_Internal(GameObject prefab, int initialCount)
        {
            var pool = ObjectPool.Instantiate(prefab, initialCount);
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