namespace UniT.Core.ObjectPool
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IObjectPoolManager
    {
        public void CreatePool(GameObject prefab, int initialCount = 1);

        public GameObject Spawn(GameObject prefab);

        public T Spawn<T>(T component) where T : Component;

        public UniTask<GameObject> Spawn(string key);

        public UniTask<T> Spawn<T>(string key) where T : Component;

        public void Recycle(GameObject instance);

        public void Recycle<T>(T component) where T : Component;
    }
}