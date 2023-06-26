namespace UniT.ObjectPool
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public interface IObjectPoolManager
    {
        public ILogger Logger { get; }
        
        public void InstantiatePool(GameObject prefab, int initialCount = 1);

        public void InstantiatePool<T>(T component, int initialCount = 1) where T : Component;

        public UniTask InstantiatePool(string key, int initialCount = 1);

        public UniTask InstantiatePool<T>(int initialCount = 1) where T : Component;

        public void DestroyPool(GameObject prefab);

        public void DestroyPool<T>(T component) where T : Component;

        public void DestroyPool(string key);

        public void DestroyPool<T>() where T : Component;

        public GameObject Spawn(GameObject prefab);

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent);

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation);

        public GameObject Spawn(GameObject prefab, Vector3 position);

        public GameObject Spawn(GameObject prefab, Quaternion rotation);

        public GameObject Spawn(GameObject prefab, Transform parent);

        public T Spawn<T>(T component) where T : Component;

        public T Spawn<T>(T component, Vector3 position, Quaternion rotation, Transform parent) where T : Component;

        public T Spawn<T>(T component, Vector3 position, Quaternion rotation) where T : Component;

        public T Spawn<T>(T component, Vector3 position) where T : Component;

        public T Spawn<T>(T component, Quaternion rotation) where T : Component;

        public T Spawn<T>(T component, Transform parent) where T : Component;

        public UniTask<GameObject> Spawn(string key);

        public UniTask<GameObject> Spawn(string key, Vector3 position, Quaternion rotation, Transform parent);

        public UniTask<GameObject> Spawn(string key, Vector3 position, Quaternion rotation);

        public UniTask<GameObject> Spawn(string key, Vector3 position);

        public UniTask<GameObject> Spawn(string key, Quaternion rotation);

        public UniTask<GameObject> Spawn(string key, Transform parent);

        public UniTask<T> Spawn<T>(string key) where T : Component;

        public UniTask<T> Spawn<T>(string key, Vector3 position, Quaternion rotation, Transform parent) where T : Component;

        public UniTask<T> Spawn<T>(string key, Vector3 position, Quaternion rotation) where T : Component;

        public UniTask<T> Spawn<T>(string key, Vector3 position) where T : Component;

        public UniTask<T> Spawn<T>(string key, Quaternion rotation) where T : Component;

        public UniTask<T> Spawn<T>(string key, Transform parent) where T : Component;

        public UniTask<T> Spawn<T>() where T : Component;

        public UniTask<T> Spawn<T>(Vector3 position, Quaternion rotation, Transform parent) where T : Component;

        public UniTask<T> Spawn<T>(Vector3 position, Quaternion rotation) where T : Component;

        public UniTask<T> Spawn<T>(Vector3 position) where T : Component;

        public UniTask<T> Spawn<T>(Quaternion rotation) where T : Component;

        public UniTask<T> Spawn<T>(Transform parent) where T : Component;
        public void       Recycle(GameObject instance);

        public void Recycle<T>(T component) where T : Component;

        public void RecycleAll(GameObject prefab);

        public void RecycleAll<T>(T component) where T : Component;

        public UniTask RecycleAll(string key);
    }
}