namespace UniT.EMC
{
    using System.Threading;
    using UnityEngine;

    public interface IEntity
    {
        public IEntityManager Manager { get; set; }

        public bool IsDestroyed { get; }

        public CancellationToken GetCancellationTokenOnRecycle();

        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();

        public GameObject gameObject { get; }

        public Transform transform { get; }

        public T[] GetComponentsInChildren<T>();
    }

    public static class EntityExtensions
    {
        public static void Recycle(this IEntity entity)
        {
            entity.Manager.Recycle(entity);
        }
    }
}