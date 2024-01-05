namespace UniT.Entities
{
    using UnityEngine;

    public interface IEntity
    {
        public IEntityManager Manager { get; set; }

        public bool IsDestroyed { get; }

        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();

        public GameObject gameObject { get; }

        public Transform transform { get; }
    }

    public interface IEntityWithoutModel : IEntity
    {
    }

    public interface IEntityWithModel<in TModel> : IEntity
    {
        public TModel Model { set; }
    }

    public static class EntityExtensions
    {
        public static void Recycle(this IEntity entity)
        {
            entity.Manager.Recycle(entity);
        }
    }
}