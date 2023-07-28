namespace UniT.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Extensions;
    using UniT.Extensions.UniTask;
    using UniT.Logging;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;

    public class ObjectPoolManager : IObjectPoolManager
    {
        public LogConfig LogConfig => this.logger.Config;

        private readonly IAssetsManager                     assetsManager;
        private readonly Transform                          poolsContainer;
        private readonly Dictionary<GameObject, ObjectPool> prefabToPool;
        private readonly Dictionary<string, ObjectPool>     keyToPool;
        private readonly Dictionary<GameObject, ObjectPool> instanceToPool;
        private readonly ILogger                            logger;

        public ObjectPoolManager(IAssetsManager assetsManager = null, ILogger logger = null)
        {
            this.assetsManager  = assetsManager ?? IAssetsManager.Default();
            this.poolsContainer = new GameObject(this.GetType().Name).DontDestroyOnLoad().transform;
            this.prefabToPool   = new();
            this.keyToPool      = new();
            this.instanceToPool = new();
            this.logger         = logger ?? ILogger.Default(this.GetType().Name);
        }

        public void InstantiatePool(GameObject prefab, int initialCount = 1)
        {
            if (this.TryInstantiatePool(prefab, initialCount)) return;
            this.logger.Warning($"Pool for prefab {prefab.name} already instantiated");
        }

        public void InstantiatePool<T>(T component, int initialCount = 1) where T : Component
        {
            this.InstantiatePool(component.gameObject, initialCount);
        }

        public UniTask InstantiatePool(string key, int initialCount = 1)
        {
            return this.TryInstantiatePool(key, initialCount)
                       .ContinueWith(success =>
                       {
                           if (success) return;
                           this.logger.Warning($"Pool for key {key} already instantiated");
                       });
        }

        public UniTask InstantiatePool<T>(int initialCount = 1) where T : Component
        {
            return this.InstantiatePool(typeof(T).GetKey(), initialCount);
        }

        public bool TryInstantiatePool(GameObject prefab, int initialCount = 1)
        {
            return this.prefabToPool.TryAdd(prefab, () => this.InstantiatePool_Internal(prefab, initialCount));
        }

        public bool TryInstantiatePool<T>(T component, int initialCount = 1) where T : Component
        {
            return this.TryInstantiatePool(component.gameObject, initialCount);
        }

        public UniTask<bool> TryInstantiatePool(string key, int initialCount = 1)
        {
            return this.keyToPool.TryAdd(key, () => this.assetsManager.Load<GameObject>(key).ContinueWith(prefab => this.InstantiatePool_Internal(prefab, initialCount)));
        }

        public UniTask<bool> TryInstantiatePool<T>(int initialCount = 1) where T : Component
        {
            return this.TryInstantiatePool(typeof(T).GetKey(), initialCount);
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
                this.logger.Warning($"Trying to destroy pool for prefab {prefab.name} that was not instantiated");
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
                this.logger.Warning($"Trying to destroy pool for key {key} that was not instantiated");
                return;
            }

            this.DestroyPool_Internal(pool);
            this.assetsManager.Unload(key);
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
            return this.Spawn(key, position, rotation, parent).GetComponent<T>();
        }

        public T Spawn<T>(Vector3? position = null, Quaternion? rotation = null, Transform parent = null) where T : Component
        {
            return this.Spawn<T>(typeof(T).GetKey(), position, rotation, parent);
        }

        public void Recycle(GameObject instance)
        {
            if (!this.instanceToPool.Remove(instance, out var pool))
            {
                this.logger.Warning($"Trying to recycle {instance.name} that was not spawned from {this.GetType().Name}");
                return;
            }

            pool.Recycle(instance);
            this.logger.Debug($"Recycled {instance.name}");
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

        private ObjectPool GetPool(GameObject prefab)
        {
            if (this.prefabToPool.TryGetValue(prefab, out var pool)) return pool;
            var exception = new InvalidOperationException($"Pool for prefab {prefab.name} was not instantiated");
            this.logger.Exception(exception);
            throw exception;
        }

        private ObjectPool GetPool(string key)
        {
            if (this.keyToPool.TryGetValue(key, out var pool)) return pool;
            var exception = new InvalidOperationException($"Pool for key {key} was not instantiated");
            this.logger.Exception(exception);
            throw exception;
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
            Object.Destroy(pool.gameObject);
            this.logger.Debug($"Destroyed {pool.gameObject.name}");
        }

        private GameObject Spawn_Internal(ObjectPool pool, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
        {
            var instance = pool.Spawn(position, rotation, parent);
            this.instanceToPool.Add(instance, pool);
            this.logger.Debug($"Spawned {instance.name}");
            return instance;
        }

        private void RecycleAll_Internal(ObjectPool pool)
        {
            pool.RecycleAll();
            this.instanceToPool.RemoveAll((_, otherPool) => otherPool == pool);
            this.logger.Debug($"Recycled all {pool.gameObject.name}");
        }
    }
}