namespace UniT.ECC.Entity
{
    using UniT.ECC.Component;
    using UnityEngine;

    public interface IEntity : IComponent
    {
        public bool IsDestroyed { get; }

        public void Recycle();

        public GameObject gameObject { get; }

        public T[] GetComponentsInChildren<T>();
    }

    public interface IEntityWithoutParams : IEntity
    {
    }

    public interface IEntityWithParams<TParams> : IEntity
    {
        public TParams Params { get; set; }
    }
}