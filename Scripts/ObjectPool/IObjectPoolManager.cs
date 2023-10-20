namespace UniT.ObjectPool
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UnityEngine;

    public interface IObjectPoolManager : IDisposable
    {
        public LogConfig LogConfig { get; }

        public void InstantiatePool(GameObject prefab, int initialCount = 1);

        public void InstantiatePool<T>(T component, int initialCount = 1) where T : Component;

        public UniTask InstantiatePool(string key, int initialCount = 1);

        public UniTask InstantiatePool<T>(int initialCount = 1) where T : Component;

        public bool IsPoolReady(GameObject prefab);

        public bool IsPoolReady<T>(T component) where T : Component;

        public bool IsPoolReady(string key);

        public bool IsPoolReady<T>() where T : Component;

        public void DestroyPool(GameObject prefab);

        public void DestroyPool<T>(T component) where T : Component;

        public void DestroyPool(string key);

        public void DestroyPool<T>() where T : Component;

        public GameObject Spawn(GameObject prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null);

        public T Spawn<T>(T component, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where T : Component;

        public GameObject Spawn(string key, Vector3 position = default, Quaternion rotation = default, Transform parent = null);

        public T Spawn<T>(string key = null, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where T : Component;

        public void Recycle(GameObject instance);

        public void Recycle<T>(T component) where T : Component;

        public void RecycleAll(GameObject prefab);

        public void RecycleAll<T>(T component) where T : Component;

        public void RecycleAll(string key);

        public void RecycleAll<T>() where T : Component;
    }
}