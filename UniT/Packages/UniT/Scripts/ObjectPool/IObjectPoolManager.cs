namespace UniT.ObjectPool
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IObjectPoolManager
    {
        public void InstantiatePool(GameObject prefab, int initialCount = 1);

        public void InstantiatePool<T>(T component, int initialCount = 1) where T : Component;

        public UniTask InstantiatePool(string key, int initialCount = 1);

        public UniTask InstantiatePool<T>(int initialCount = 1) where T : Component;

        public ObjectPool GetPool(GameObject prefab);

        public ObjectPool GetPool<T>(T component) where T : Component;

        public UniTask<ObjectPool> GetPool(string key);

        public UniTask<ObjectPool> GetPool<T>() where T : Component;

        public void DestroyPool(GameObject prefab);

        public void DestroyPool<T>(T component) where T : Component;

        public void DestroyPool(string key);

        public void DestroyPool<T>() where T : Component;
    }
}