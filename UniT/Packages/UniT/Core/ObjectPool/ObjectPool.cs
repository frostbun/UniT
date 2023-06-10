namespace UniT.Core.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using UniT.Core.Extensions;
    using UnityEngine;

    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject          prefab;
        private                  Queue<GameObject>   pooledObjects  = new();
        private                  HashSet<GameObject> spawnedObjects = new();

        public static ObjectPool Instantiate(GameObject prefab, int initialCount)
        {
            var pool = new GameObject($"{prefab.name} Pool").AddComponent<ObjectPool>();
            pool.prefab         = prefab;
            pool.pooledObjects  = IterTools.Repeat(() => Instantiate(pool.prefab), initialCount).ToQueue();
            pool.spawnedObjects = new();
            DontDestroyOnLoad(pool);
            return pool;
        }

        public GameObject Spawn()
        {
            var instance = this.pooledObjects.DequeueOrDefault(() => Instantiate(this.prefab));
            this.spawnedObjects.Add(instance);
            this.transform.SetParent(null);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void Recycle(GameObject instance)
        {
            if (!this.spawnedObjects.Contains(instance)) throw new InvalidOperationException($"{instance.name} does not spawn from {this.gameObject.name}");
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(this.transform);
            this.spawnedObjects.Remove(instance);
            this.pooledObjects.Enqueue(instance);
        }
    }
}