namespace UniT.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UniT.Extensions;
    using UnityEngine;

    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        private readonly Queue<GameObject>   _pooledObjects  = new();
        private readonly HashSet<GameObject> _spawnedObjects = new();

        public static ObjectPool Instantiate(GameObject prefab, int initialCount)
        {
            var pool = new GameObject($"{prefab.name} Pool").AddComponent<ObjectPool>();
            pool._prefab = prefab;
            IterTools.Repeat(() =>
            {
                var instance = Instantiate(pool._prefab, pool.transform);
                instance.SetActive(false);
                pool._pooledObjects.Enqueue(instance);
            }, initialCount);
            return pool;
        }

        public GameObject Spawn(Vector3? position = null, Quaternion? rotation = null, Transform parent = null, bool worldPositionStays = true)
        {
            var instance = this._pooledObjects.DequeueOrDefault(() => Instantiate(this._prefab, this.transform));
            this._spawnedObjects.Add(instance);
            instance.transform.SetPositionAndRotation(position ?? Vector3.zero, rotation ?? Quaternion.identity);
            instance.transform.SetParent(parent, worldPositionStays);
            instance.SetActive(true);
            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Spawn<T>(Vector3? position = null, Quaternion? rotation = null, Transform parent = null, bool worldPositionStays = true) where T : Component
        {
            return this.Spawn(position, rotation, parent, worldPositionStays).GetComponent<T>();
        }

        public void Recycle(GameObject instance)
        {
            if (!this._spawnedObjects.Remove(instance)) throw new InvalidOperationException($"{instance.name} does not spawn from {this.gameObject.name}");
            instance.SetActive(false);
            instance.transform.SetParent(this.transform);
            instance.GetComponentsInChildren<IRecyclable>().ForEach(recyclable => recyclable.Recycle());
            this._pooledObjects.Enqueue(instance);
        }

        public void Recycle<T>(T component) where T : Component
        {
            this.Recycle(component.gameObject);
        }

        public void RecycleAll()
        {
            this._spawnedObjects.ToArray().ForEach(this.Recycle);
        }
    }
}