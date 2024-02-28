namespace UniT.Entities
{
    using UnityEngine;

    public interface IEntity : IComponent
    {
        public bool IsDestroyed { get; }

        public void Recycle();

        public GameObject gameObject { get; }

        public T[] GetComponentsInChildren<T>();
    }

    public interface IEntityWithoutModel : IEntity
    {
    }

    public interface IEntityWithModel<in TModel> : IEntity
    {
        public TModel Model { set; }
    }
}