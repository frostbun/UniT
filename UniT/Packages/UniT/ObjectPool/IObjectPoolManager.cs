namespace UniT.ObjectPool
{
    using UnityEngine;

    public interface IObjectPoolManager
    {
        public void CreatePool(GameObject prefab, int initialCount = 1);

        public GameObject Spawn(GameObject prefab);

        public void Recycle(GameObject instance);
    }
}