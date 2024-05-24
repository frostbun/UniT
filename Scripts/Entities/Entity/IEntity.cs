#nullable enable
namespace UniT.Entities.Entity
{
    using UniT.Entities.Component;

    public interface IEntity : IComponent
    {
        public T[] GetComponentsInChildren<T>();

        public void Recycle();
    }

    public interface IEntityWithoutParams : IEntity
    {
    }

    public interface IEntityWithParams<TParams> : IEntity
    {
        public TParams Params { get; set; }
    }
}