namespace UniT.ObjectPool
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Extensions;
    using UniT.Extensions.UniTask;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class ObjectPoolManager : IObjectPoolManager
    {
        public ILogger Logger { get; }

        private readonly IAddressableManager                addressableManager;
        private readonly Dictionary<GameObject, ObjectPool> prefabToPool;
        private readonly Dictionary<string, ObjectPool>     keyToPool;
        private readonly Dictionary<GameObject, ObjectPool> instanceToPool;

        public ObjectPoolManager(IAddressableManager addressableManager, ILogger logger)
        {
            this.addressableManager = addressableManager;
            this.prefabToPool       = new();
            this.keyToPool          = new();
            this.instanceToPool     = new();
            this.Logger             = logger;
            this.Logger.Info($"{this.GetType().Name} instantiated");
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

        public void DestroyPool(GameObject prefab)
        {
            if (!this.prefabToPool.Remove(prefab, out var pool))
            {
                this.Logger.Warning($"Trying to destroy pool for prefab {prefab.name} that was not instantiated");
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
                this.Logger.Warning($"Trying to destroy pool for key {key} that was not instantiated");
                return;
            }

            this.DestroyPool_Internal(pool);
            this.addressableManager.Unload(key);
        }

        public void DestroyPool<T>() where T : Component
        {
            this.DestroyPool(typeof(T).GetKeyAttribute());
        }

        public GameObject Spawn(GameObject prefab)
        {
            return this.Spawn_Internal(this.GetPool(prefab));
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return this.Spawn_Internal(this.GetPool(prefab), position, rotation, parent);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return this.Spawn_Internal(this.GetPool(prefab), position, rotation);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position)
        {
            return this.Spawn_Internal(this.GetPool(prefab), position);
        }

        public GameObject Spawn(GameObject prefab, Quaternion rotation)
        {
            return this.Spawn_Internal(this.GetPool(prefab), rotation);
        }

        public GameObject Spawn(GameObject prefab, Transform parent)
        {
            return this.Spawn_Internal(this.GetPool(prefab), parent);
        }

        public T Spawn<T>(T component) where T : Component
        {
            return this.Spawn(component.gameObject).GetComponent<T>();
        }

        public T Spawn<T>(T component, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            return this.Spawn(component.gameObject, position, rotation, parent).GetComponent<T>();
        }

        public T Spawn<T>(T component, Vector3 position, Quaternion rotation) where T : Component
        {
            return this.Spawn(component.gameObject, position, rotation).GetComponent<T>();
        }

        public T Spawn<T>(T component, Vector3 position) where T : Component
        {
            return this.Spawn(component.gameObject, position).GetComponent<T>();
        }

        public T Spawn<T>(T component, Quaternion rotation) where T : Component
        {
            return this.Spawn(component.gameObject, rotation).GetComponent<T>();
        }

        public T Spawn<T>(T component, Transform parent) where T : Component
        {
            return this.Spawn(component.gameObject, parent).GetComponent<T>();
        }

        public UniTask<GameObject> Spawn(string key)
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool));
        }

        public UniTask<GameObject> Spawn(string key, Vector3 position, Quaternion rotation, Transform parent)
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool, position, rotation, parent));
        }

        public UniTask<GameObject> Spawn(string key, Vector3 position, Quaternion rotation)
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool, position, rotation));
        }

        public UniTask<GameObject> Spawn(string key, Vector3 position)
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool, position));
        }

        public UniTask<GameObject> Spawn(string key, Quaternion rotation)
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool, rotation));
        }

        public UniTask<GameObject> Spawn(string key, Transform parent)
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool, parent));
        }

        public UniTask<T> Spawn<T>(string key) where T : Component
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool).GetComponent<T>());
        }

        public UniTask<T> Spawn<T>(string key, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool, position, rotation, parent).GetComponent<T>());
        }

        public UniTask<T> Spawn<T>(string key, Vector3 position, Quaternion rotation) where T : Component
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool, position, rotation).GetComponent<T>());
        }

        public UniTask<T> Spawn<T>(string key, Vector3 position) where T : Component
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool, position).GetComponent<T>());
        }

        public UniTask<T> Spawn<T>(string key, Quaternion rotation) where T : Component
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool, rotation).GetComponent<T>());
        }

        public UniTask<T> Spawn<T>(string key, Transform parent) where T : Component
        {
            return this.GetPool(key).ContinueWith(pool => this.Spawn_Internal(pool, parent).GetComponent<T>());
        }

        public UniTask<T> Spawn<T>() where T : Component
        {
            return this.Spawn<T>(typeof(T).GetKeyAttribute());
        }

        public UniTask<T> Spawn<T>(Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            return this.Spawn<T>(typeof(T).GetKeyAttribute(), position, rotation, parent);
        }

        public UniTask<T> Spawn<T>(Vector3 position, Quaternion rotation) where T : Component
        {
            return this.Spawn<T>(typeof(T).GetKeyAttribute(), position, rotation);
        }

        public UniTask<T> Spawn<T>(Vector3 position) where T : Component
        {
            return this.Spawn<T>(typeof(T).GetKeyAttribute(), position);
        }

        public UniTask<T> Spawn<T>(Quaternion rotation) where T : Component
        {
            return this.Spawn<T>(typeof(T).GetKeyAttribute(), rotation);
        }

        public UniTask<T> Spawn<T>(Transform parent) where T : Component
        {
            return this.Spawn<T>(typeof(T).GetKeyAttribute(), parent);
        }

        public void Recycle(GameObject instance)
        {
            if (!this.instanceToPool.Remove(instance, out var pool))
            {
                this.Logger.Warning($"Trying to recycle {instance.name} that was not spawned from {this.GetType().Name}");
                return;
            }

            pool.Recycle(instance);
            this.Logger.Debug($"Recycled {instance.name}");
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

        public UniTask RecycleAll(string key)
        {
            return this.GetPool(key).ContinueWith(this.RecycleAll_Internal);
        }

        public UniTask RecycleAll<T>() where T : Component
        {
            return this.RecycleAll(typeof(T).GetKeyAttribute());
        }

        private ObjectPool GetPool(GameObject prefab)
        {
            this.InstantiatePool(prefab);
            return this.prefabToPool[prefab];
        }

        private UniTask<ObjectPool> GetPool(string key)
        {
            return this.InstantiatePool(key).ContinueWith(() => this.keyToPool[key]);
        }

        private ObjectPool InstantiatePool_Internal(GameObject prefab, int initialCount = 1)
        {
            var pool = ObjectPool.Instantiate(prefab, initialCount);
            this.Logger.Debug($"Instantiated {pool.gameObject.name}");
            return pool;
        }

        private void DestroyPool_Internal(ObjectPool pool)
        {
            this.RecycleAll_Internal(pool);
            Object.Destroy(pool.gameObject);
            this.Logger.Debug($"Destroyed {pool.gameObject.name}");
        }

        private GameObject Spawn_Internal(ObjectPool pool)
        {
            var instance = pool.Spawn();
            this.instanceToPool.Add(instance, pool);
            this.Logger.Debug($"Spawned {instance.name}");
            return instance;
        }

        private GameObject Spawn_Internal(ObjectPool pool, Vector3 position, Quaternion rotation, Transform parent)
        {
            var instance = pool.Spawn(position, rotation, parent);
            this.instanceToPool.Add(instance, pool);
            this.Logger.Debug($"Spawned {instance.name}");
            return instance;
        }

        private GameObject Spawn_Internal(ObjectPool pool, Vector3 position, Quaternion rotation)
        {
            var instance = pool.Spawn(position, rotation);
            this.instanceToPool.Add(instance, pool);
            this.Logger.Debug($"Spawned {instance.name}");
            return instance;
        }

        private GameObject Spawn_Internal(ObjectPool pool, Vector3 position)
        {
            var instance = pool.Spawn(position);
            this.instanceToPool.Add(instance, pool);
            this.Logger.Debug($"Spawned {instance.name}");
            return instance;
        }

        private GameObject Spawn_Internal(ObjectPool pool, Quaternion rotation)
        {
            var instance = pool.Spawn(rotation);
            this.instanceToPool.Add(instance, pool);
            this.Logger.Debug($"Spawned {instance.name}");
            return instance;
        }

        private GameObject Spawn_Internal(ObjectPool pool, Transform parent)
        {
            var instance = pool.Spawn(parent);
            this.instanceToPool.Add(instance, pool);
            this.Logger.Debug($"Spawned {instance.name}");
            return instance;
        }

        private void RecycleAll_Internal(ObjectPool pool)
        {
            pool.RecycleAll();
            this.instanceToPool.RemoveAll((_, otherPool) => otherPool == pool);
            this.Logger.Debug($"Recycled all {pool.gameObject.name}");
        }
    }
}