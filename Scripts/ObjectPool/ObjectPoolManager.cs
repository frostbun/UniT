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

        private readonly IAssetManager                         _assetManager;
        private readonly Transform                             _poolsContainer;
        private readonly Dictionary<GameObject, ObjectPool>    _prefabToPool;
        private readonly Dictionary<string, ObjectPool>        _keyToPool;
        private readonly Dictionary<GameObject, ObjectPool>    _instanceToPool;
        private readonly Dictionary<GameObject, IRecyclable[]> _recyclables;
        private readonly ILogger                               _logger;

        [Preserve]
        public ObjectPoolManager(IAssetManager assetManager = null, ILogger logger = null)
        {
            this._assetManager   = assetManager ?? IAssetManager.Default();
            this._poolsContainer = new GameObject(this.GetType().Name).DontDestroyOnLoad().transform;
            this._prefabToPool   = new();
            this._keyToPool      = new();
            this._instanceToPool = new();
            this._recyclables    = new();
            this._logger         = logger ?? ILogger.Default(this.GetType().Name);
            this._logger.Debug("Constructed");
        }

        #endregion

        #region Finalizer

        ~ObjectPoolManager()
        {
            this.Dispose();
            this._logger.Debug("Finalized");
        }

        public void Dispose()
        {
            this._prefabToPool.Keys.SafeForEach(this.DestroyPool);
            this._keyToPool.Keys.SafeForEach(this.DestroyPool);
            Object.Destroy(this._poolsContainer.gameObject);
            this._logger.Debug("Disposed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this._logger.Config;

        public void InstantiatePool(GameObject prefab, int initialCount = 1)
        {
            if (this._prefabToPool.TryAdd(prefab, () => this.InstantiatePool_Internal(prefab, initialCount))) return;
            this._logger.Warning($"Pool for {prefab.name} already instantiated");
        }

        public void InstantiatePool<T>(T component, int initialCount = 1) where T : Component
        {
            this.InstantiatePool(component.gameObject, initialCount);
        }

        public UniTask InstantiatePool(string key, int initialCount = 1)
        {
            return this._keyToPool.TryAddAsync(key, () => this._assetManager.Load<GameObject>(key).ContinueWith(prefab => this.InstantiatePool_Internal(prefab, initialCount)))
                .ContinueWith(isSuccess =>
                {
                    if (isSuccess) return;
                    this._logger.Warning($"Pool for {key} already instantiated");
                });
        }

        public UniTask InstantiatePool<T>(int initialCount = 1) where T : Component
        {
            return this.InstantiatePool(typeof(T).GetKey(), initialCount);
        }

        public bool IsPoolReady(GameObject prefab)
        {
            return this._prefabToPool.ContainsKey(prefab);
        }

        public bool IsPoolReady<T>(T component) where T : Component
        {
            return this.IsPoolReady(component.gameObject);
        }

        public bool IsPoolReady(string key)
        {
            return this._keyToPool.ContainsKey(key);
        }

        public bool IsPoolReady<T>() where T : Component
        {
            return this.IsPoolReady(typeof(T).GetKey());
        }

        public void DestroyPool(GameObject prefab)
        {
            if (!this._prefabToPool.Remove(prefab, out var pool))
            {
                this._logger.Warning($"Trying to destroy pool for {prefab.name} that was not instantiated");
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
            if (!this._keyToPool.Remove(key, out var pool))
            {
                this._logger.Warning($"Trying to destroy pool for {key} that was not instantiated");
                return;
            }
            this.DestroyPool_Internal(pool);
            this._assetManager.Unload(key);
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
            if (!this._instanceToPool.Remove(instance, out var pool))
            {
                this._logger.Warning($"Trying to recycle {instance.name} that was not spawned from {this.GetType().Name}");
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
            return this._prefabToPool.GetOrDefault(prefab)
                ?? throw new InvalidOperationException($"Pool for {prefab.name} was not instantiated");
        }

        private ObjectPool GetPool(string key)
        {
            return this._keyToPool.GetOrDefault(key)
                ?? throw new InvalidOperationException($"Pool for {key} was not instantiated");
        }

        private ObjectPool InstantiatePool_Internal(GameObject prefab, int initialCount)
        {
            var pool = ObjectPool.Instantiate(prefab, initialCount);
            pool.transform.SetParent(this._poolsContainer);
            this._logger.Debug($"Instantiated {pool.gameObject.name}");
            return pool;
        }

        private void DestroyPool_Internal(ObjectPool pool)
        {
            this.RecycleAll_Internal(pool);
            pool.GetComponentsInChildren<Transform>().ForEach(transform => this._recyclables.Remove(transform.gameObject));
            Object.Destroy(pool.gameObject);
            this._logger.Debug($"Destroyed {pool.gameObject.name}");
        }

        private void Spawn_Internal(GameObject instance, ObjectPool pool)
        {
            this._instanceToPool.Add(instance, pool);
            this._recyclables.GetOrAdd(instance, () =>
            {
                var recyclables = instance.GetComponentsInChildren<IRecyclable>(true);
                recyclables.ForEach(recyclable =>
                {
                    recyclable.Manager = this;
                    recyclable.OnInstantiate();
                });
                this._logger.Debug($"Instantiated {instance.name}");
                return recyclables;
            }).ForEach(component => component.OnSpawn());
            this._logger.Debug($"Spawned {instance.name}");
        }

        private void Recycle_Internal(GameObject instance, ObjectPool pool)
        {
            pool.Recycle(instance);
            this._recyclables[instance].ForEach(recyclable => recyclable.OnRecycle());
            if (!instance)
            {
                this._recyclables.Remove(instance);
                this._logger.Warning($"Trying to recycle {instance.name} that was already destroyed");
                return;
            }
            this._logger.Debug($"Recycled {instance.name}");
        }

        private void RecycleAll_Internal(ObjectPool pool)
        {
            this._instanceToPool.RemoveAll((instance, otherPool) =>
            {
                if (otherPool != pool) return false;
                this.Recycle_Internal(instance, pool);
                return true;
            });
            this._logger.Debug($"Recycled all {pool.gameObject.name}");
        }

        #endregion
    }
}