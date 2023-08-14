namespace UniT.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;

    public class ObjectPoolManager : IObjectPoolManager
    {
        #region Constructor

        private readonly IAssetManager                      _assetManager;
        private readonly Transform                          _poolsContainer;
        private readonly Dictionary<GameObject, ObjectPool> _prefabToPool;
        private readonly Dictionary<string, ObjectPool>     _keyToPool;
        private readonly Dictionary<GameObject, ObjectPool> _instanceToPool;
        private readonly ILogger                            _logger;

        [Preserve]
        public ObjectPoolManager(IAssetManager assetManager = null, ILogger logger = null)
        {
            this._assetManager   = assetManager ?? IAssetManager.Default();
            this._poolsContainer = new GameObject(this.GetType().Name).DontDestroyOnLoad().transform;
            this._prefabToPool   = new();
            this._keyToPool      = new();
            this._instanceToPool = new();
            this._logger         = logger ?? ILogger.Default(this.GetType().Name);
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
            return this._keyToPool.TryAdd(key, () => this._assetManager.Load<GameObject>(key).ContinueWith(prefab => this.InstantiatePool_Internal(prefab, initialCount)))
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

        public GameObject Spawn(GameObject prefab, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
        {
            return this.Spawn_Internal(this.GetPool(prefab), position, rotation, parent);
        }

        public T Spawn<T>(T component, Vector3? position = null, Quaternion? rotation = null, Transform parent = null) where T : Component
        {
            return this.Spawn(component.gameObject, position, rotation, parent).GetComponent<T>();
        }

        public GameObject Spawn(string key, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
        {
            return this.Spawn_Internal(this.GetPool(key), position, rotation, parent);
        }

        public T Spawn<T>(string key, Vector3? position = null, Quaternion? rotation = null, Transform parent = null) where T : Component
        {
            return this.Spawn(key ?? typeof(T).GetKey(), position, rotation, parent).GetComponent<T>();
        }

        public void Recycle(GameObject instance)
        {
            if (!this._instanceToPool.Remove(instance, out var pool))
            {
                this._logger.Warning($"Trying to recycle {instance.name} that was not spawned from {this.GetType().Name}");
                return;
            }
            pool.Recycle(instance);
            this._logger.Debug($"Recycled {instance.name}");
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ObjectPool GetPool(GameObject prefab)
        {
            if (this._prefabToPool.TryGetValue(prefab, out var pool)) return pool;
            var exception = new InvalidOperationException($"Pool for prefab {prefab.name} was not instantiated");
            this._logger.Exception(exception);
            throw exception;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ObjectPool GetPool(string key)
        {
            if (this._keyToPool.TryGetValue(key, out var pool)) return pool;
            var exception = new InvalidOperationException($"Pool for key {key} was not instantiated");
            this._logger.Exception(exception);
            throw exception;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ObjectPool InstantiatePool_Internal(GameObject prefab, int initialCount)
        {
            var pool = ObjectPool.Instantiate(prefab, initialCount);
            pool.transform.SetParent(this._poolsContainer);
            this._logger.Debug($"Instantiated {pool.gameObject.name}");
            return pool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DestroyPool_Internal(ObjectPool pool)
        {
            this.RecycleAll_Internal(pool);
            Object.Destroy(pool.gameObject);
            this._logger.Debug($"Destroyed {pool.gameObject.name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObject Spawn_Internal(ObjectPool pool, Vector3? position, Quaternion? rotation, Transform parent)
        {
            var instance = pool.Spawn(position, rotation, parent);
            this._instanceToPool.Add(instance, pool);
            this._logger.Debug($"Spawned {instance.name}");
            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecycleAll_Internal(ObjectPool pool)
        {
            pool.RecycleAll();
            this._instanceToPool.RemoveAll((_, otherPool) => otherPool == pool);
            this._logger.Debug($"Recycled all {pool.gameObject.name}");
        }

        #endregion
    }
}