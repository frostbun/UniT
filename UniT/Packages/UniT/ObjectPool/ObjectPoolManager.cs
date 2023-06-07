namespace UniT.ObjectPool
{
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine;

    public class ObjectPoolManager : IObjectPoolManager
    {
        private readonly ILogger                            logger;
        private readonly Dictionary<GameObject, ObjectPool> prefabToPool;
        private readonly Dictionary<GameObject, ObjectPool> instanceToPool;

        public ObjectPoolManager(ILogger logger)
        {
            this.logger = logger;
            var pools                   = Object.FindObjectsOfType<ObjectPool>(true);
            var prefabFieldInfo         = typeof(ObjectPool).GetField("prefab");
            var spawnedObjectsFieldInfo = typeof(ObjectPool).GetField("spawnedObjects");
            this.prefabToPool   = pools.ToDictionaryOneToOne(pool => (GameObject)prefabFieldInfo.GetValue(pool), pool => pool);
            this.instanceToPool = pools.ToDictionaryManyToOne(pool => (HashSet<GameObject>)spawnedObjectsFieldInfo.GetValue(pool), pool => pool);
            this.logger.Log($"{this.GetType().Name} instantiated with {this.prefabToPool.Count} pool", Color.green);
        }

        public void CreatePool(GameObject prefab, int initialCount = 1)
        {
            this.prefabToPool.TryAdd(prefab, () => ObjectPool.Instantiate(prefab, initialCount));
        }

        private ObjectPool GetPool(GameObject prefab)
        {
            this.CreatePool(prefab);
            return this.prefabToPool[prefab];
        }

        public GameObject Spawn(GameObject prefab)
        {
            var pool     = this.GetPool(prefab);
            var instance = pool.Spawn();
            this.instanceToPool[instance] = pool;
            return instance;
        }

        public void Recycle(GameObject instance)
        {
            this.instanceToPool[instance].Recycle(instance);
        }
    }
}