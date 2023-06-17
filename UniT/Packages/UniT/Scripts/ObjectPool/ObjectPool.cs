namespace UniT.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine;

    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject          prefab;
        private                  Queue<GameObject>   pooledObjects  = new();
        private                  HashSet<GameObject> spawnedObjects = new();

        public static ObjectPool Instantiate(GameObject prefab, int initialCount)
        {
            var pool = new GameObject($"{prefab.name} Pool").AddComponent<ObjectPool>();
            pool.prefab = prefab;
            pool.pooledObjects = IterTools.Repeat(() =>
            {
                var instance = Instantiate(pool.prefab, pool.transform);
                instance.gameObject.SetActive(false);
                return instance;
            }, initialCount).ToQueue();
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

        public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent)
        {
            var instance = this.Spawn();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.transform.SetParent(parent);
            return instance;
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            var instance = this.Spawn();
            instance.transform.SetPositionAndRotation(position, rotation);
            return instance;
        }

        public GameObject Spawn(Vector3 position)
        {
            var instance = this.Spawn();
            instance.transform.position = position;
            return instance;
        }

        public GameObject Spawn(Quaternion rotation)
        {
            var instance = this.Spawn();
            instance.transform.rotation = rotation;
            return instance;
        }

        public GameObject Spawn(Transform parent)
        {
            var instance = this.Spawn();
            instance.transform.SetParent(parent);
            return instance;
        }

        public T Spawn<T>() where T : Component
        {
            return this.Spawn().GetComponent<T>();
        }

        public T Spawn<T>(Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            return this.Spawn(position, rotation, parent).GetComponent<T>();
        }

        public T Spawn<T>(Vector3 position, Quaternion rotation) where T : Component
        {
            return this.Spawn(position, rotation).GetComponent<T>();
        }

        public T Spawn<T>(Vector3 position) where T : Component
        {
            return this.Spawn(position).GetComponent<T>();
        }

        public T Spawn<T>(Quaternion rotation) where T : Component
        {
            return this.Spawn(rotation).GetComponent<T>();
        }

        public T Spawn<T>(Transform parent) where T : Component
        {
            return this.Spawn(parent).GetComponent<T>();
        }

        public void Recycle(GameObject instance)
        {
            if (!this.spawnedObjects.Contains(instance)) throw new InvalidOperationException($"{instance.name} does not spawn from {this.gameObject.name}");
            this.Recycle_Internal(instance);
            this.spawnedObjects.Remove(instance);
        }

        public void Recycle<T>(T component) where T : Component
        {
            this.Recycle(component.gameObject);
        }

        public void RecycleAll()
        {
            this.spawnedObjects.ForEach(this.Recycle_Internal);
            this.spawnedObjects.Clear();
        }

        private void Recycle_Internal(GameObject instance)
        {
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(this.transform);
            this.pooledObjects.Enqueue(instance);
        }
    }
}