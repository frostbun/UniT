namespace UniT.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine;

    public sealed class ObjectPool : MonoBehaviour
    {
        #region Constructor

        [SerializeField] private GameObject prefab;

        private new      Transform           transform;
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

        private void Awake()
        {
            this.transform = ((Component)this).transform;
        }

        #endregion

        #region Public

        public GameObject Spawn(Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var instance = this.pooledObjects.DequeueOrDefault(() => Instantiate(this.prefab, this.transform));
            this.spawnedObjects.Add(instance);
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.transform.SetParent(parent);
            instance.SetActive(true);
            return instance;
        }

        public T Spawn<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            var instance = this.Spawn(position, rotation, parent);
            return instance.GetComponent<T>() ?? throw new InvalidOperationException($"Component {typeof(T).Name} not found in GameObject {instance.name}");
        }

        public void Recycle(GameObject instance)
        {
            if (!this.spawnedObjects.Remove(instance)) throw new InvalidOperationException($"{instance.name} was not spawned from {this.gameObject.name}");
            if (!instance) return;
            this.pooledObjects.Enqueue(instance);
            instance.SetActive(false);
            instance.transform.SetParent(this.transform);
        }

        public void Recycle<T>(T component) where T : Component
        {
            this.Recycle(component.gameObject);
        }

        public void RecycleAll()
        {
            this.spawnedObjects.SafeForEach(this.Recycle);
        }

        #endregion
    }
}