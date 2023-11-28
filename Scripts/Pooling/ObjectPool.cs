namespace UniT.Pooling
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

        public static ObjectPool Instantiate(GameObject prefab)
        {
            var pool = new GameObject($"{prefab.name} pool").AddComponent<ObjectPool>();
            pool.prefab = prefab;
            prefab.gameObject.SetActive(false);
            return pool;
        }

        private void Awake()
        {
            this.transform = ((Component)this).transform;
        }

        #endregion

        #region Public

        public void Load(int count)
        {
            while (this.pooledObjects.Count < count) this.pooledObjects.Enqueue(Instantiate(this.prefab, this.transform));
        }

        public GameObject Spawn(Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var instance = this.pooledObjects.DequeueOrDefault(() => Instantiate(this.prefab, this.transform));
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.transform.SetParent(parent);
            instance.SetActive(true);
            this.spawnedObjects.Add(instance);
            return instance;
        }

        public T Spawn<T>(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return this.Spawn(position, rotation, parent).GetComponent<T>()
                ?? throw new InvalidOperationException($"Component {typeof(T).Name} not found in GameObject {this.prefab.name}");
        }

        public void Recycle(GameObject instance)
        {
            if (!this.spawnedObjects.Remove(instance)) throw new InvalidOperationException($"{instance.name} was not spawned from {this.gameObject.name}");
            if (!instance) return;
            instance.SetActive(false);
            instance.transform.SetParent(this.transform);
            this.pooledObjects.Enqueue(instance);
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