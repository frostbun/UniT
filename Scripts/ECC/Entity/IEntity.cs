namespace UniT.ECC.Entity
{
    using UniT.ECC.Component;

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