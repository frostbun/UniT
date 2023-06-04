namespace UniT.ObjectPool
{
    using System.Collections.Generic;
    using FrostBurn.ObjectPool;
    using UniT.Extensions;
    using UnityEngine;

    public class ObjectPool<T> : MonoBehaviour, IObjectPool where T : Component, IRecyclable
    {
        private T        prefab;
        private Queue<T> pooledObjects;

        public void Initialize(T prefab, int initialCount)
        {
            this.prefab          = prefab;
            this.pooledObjects   = IterTools.Repeat(() => Instantiate(this.prefab), initialCount).ToQueue();
            this.gameObject.name = $"{this.prefab.name} Pool";
        }

        public T Spawn()
        {
            var obj = this.pooledObjects.DequeueOrDefault(() => Instantiate(this.prefab));
            this.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Recycle(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(this.transform);
            obj.Recycle();
            this.pooledObjects.Enqueue(obj);
        }

        private void OnDestroy()
        {
            this.pooledObjects.ForEach(Destroy);
        }
    }
}