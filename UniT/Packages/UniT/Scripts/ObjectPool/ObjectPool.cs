namespace UniT.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine;

    public class ObjectPool : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab;

        private readonly Queue<GameObject>   pooledObjects  = new();
        private readonly HashSet<GameObject> spawnedObjects = new();

        public static ObjectPool Instantiate(GameObject prefab, int initialCount)
        {
            var pool = new GameObject($"{prefab.name} Pool").AddComponent<ObjectPool>();
            pool.prefab = prefab;
            IterTools.Repeat(() =>
            {
                var instance = Instantiate(pool.prefab, pool.transform);
                instance.SetActive(false);
                pool.pooledObjects.Enqueue(instance);
            }, initialCount);
            return pool;
        }

        public GameObject Spawn(Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
        {
            var instance = this.pooledObjects.DequeueOrDefault(() => Instantiate(this.prefab, this.transform));
            this.spawnedObjects.Add(instance);
            instance.transform.SetPositionAndRotation(position ?? Vector3.zero, rotation ?? Quaternion.identity);
            instance.transform.SetParent(parent);
            instance.SetActive(true);
            return instance;
        }

        public T Spawn<T>(Vector3? position = null, Quaternion? rotation = null, Transform parent = null) where T : Component
        {
            return this.Spawn(position, rotation, parent).GetComponent<T>();
        }

        public void Recycle(GameObject instance)
        {
            if (!this.spawnedObjects.Remove(instance)) throw new InvalidOperationException($"{instance.name} does not spawn from {this.gameObject.name}");
            instance.SetActive(false);
            instance.transform.SetParent(this.transform);
            instance.GetComponentsInChildren<IRecyclable>().ForEach(recyclable => recyclable.Recycle());
            this.pooledObjects.Enqueue(instance);
        }

        public void Recycle<T>(T component) where T : Component
        {
            this.Recycle(component.gameObject);
        }

        public void RecycleAll()
        {
            this.spawnedObjects.ToArray().ForEach(this.Recycle);
        }
    }
}