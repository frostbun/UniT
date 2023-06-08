namespace UniT.Core.ObjectPool
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Core.Addressables;
    using UniT.Core.Extensions;
    using UnityEngine;
    using ILogger = UniT.Core.Logging.ILogger;

    public class ObjectPoolManager : IObjectPoolManager
    {
        private readonly IAddressableManager                addressableManager;
        private readonly ILogger                            logger;
        private readonly Dictionary<GameObject, ObjectPool> prefabToPool;
        private readonly Dictionary<GameObject, ObjectPool> instanceToPool;

        public ObjectPoolManager(IAddressableManager addressableManager, ILogger logger)
        {
            this.addressableManager = addressableManager;
            this.logger             = logger;
            var pools                   = Object.FindObjectsOfType<ObjectPool>(true);
            var prefabFieldInfo         = typeof(ObjectPool).GetField("prefab");
            var spawnedObjectsFieldInfo = typeof(ObjectPool).GetField("spawnedObjects");
            this.prefabToPool   = pools.ToDictionaryOneToOne(pool => (GameObject)prefabFieldInfo.GetValue(pool), pool => pool);
            this.instanceToPool = pools.ToDictionaryManyToOne(pool => (HashSet<GameObject>)spawnedObjectsFieldInfo.GetValue(pool), pool => pool);
            this.logger.Log($"{this.GetType().Name} instantiated with {this.prefabToPool.Count} pre-created pool", Color.green);
        }

        public void CreatePool(GameObject prefab, int initialCount = 1)
        {
            this.prefabToPool.TryAdd(prefab, () =>
            {
                var pool = ObjectPool.Instantiate(prefab, initialCount);
                this.logger.Log($"Created {pool.gameObject.name}");
                return pool;
            });
        }

        public void CreatePool<T>(T component, int initialCount = 1) where T : Component
        {
            this.CreatePool(component.gameObject, initialCount);
        }

        public UniTask CreatePool(string key, int initialCount = 1)
        {
            return this.addressableManager.Load<GameObject>(key, cache: true).ContinueWith(prefab => this.CreatePool(prefab, initialCount));
        }

        public UniTask CreatePool<T>(int initialCount = 1) where T : Component
        {
            return this.CreatePool(typeof(T).GetKeyAttribute(), initialCount);
        }

        public GameObject Spawn(GameObject prefab)
        {
            var pool     = this.GetPool(prefab);
            var instance = pool.Spawn();
            this.instanceToPool[instance] = pool;
            this.logger.Log($"Spawned {prefab.name}");
            return instance;
        }

        public T Spawn<T>(T component) where T : Component
        {
            return this.Spawn(component.gameObject).GetComponent<T>();
        }

        public UniTask<GameObject> Spawn(string key)
        {
            return this.addressableManager.Load<GameObject>(key, cache: true).ContinueWith(this.Spawn);
        }

        public UniTask<T> Spawn<T>(string key) where T : Component
        {
            return this.Spawn(key).ContinueWith(instance => instance.GetComponent<T>());
        }

        public UniTask<T> Spawn<T>() where T : Component
        {
            return this.Spawn<T>(typeof(T).GetKeyAttribute());
        }

        public void Recycle(GameObject instance)
        {
            this.instanceToPool.Remove(instance, out var pool);
            pool.Recycle(instance);
            this.logger.Log($"Recycled {instance.name}");
        }

        public void Recycle<T>(T component) where T : Component
        {
            this.Recycle(component.gameObject);
        }

        private ObjectPool GetPool(GameObject prefab)
        {
            this.CreatePool(prefab);
            return this.prefabToPool[prefab];
        }
    }
}