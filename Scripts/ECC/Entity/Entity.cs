namespace UniT.ECC.Entity
{
    using UniT.ECC.Component;

    public abstract class BaseEntity : Component, IEntity
    {
        bool IEntity.IsDestroyed => !this;

        public void Recycle() => this.Manager.Recycle(this);
    }

    public abstract class Entity : BaseEntity, IEntityWithoutParams
    {
    }

    public abstract class Entity<TParams> : BaseEntity, IEntityWithParams<TParams>
    {
        TParams IEntityWithParams<TParams>.Params { get => this.Params; set => this.Params = value; }

        protected TParams Params { get; private set; }
    }
}