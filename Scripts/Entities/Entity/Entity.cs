#nullable enable
namespace UniT.Entities.Entity
{
    using UniT.Entities.Component;

    public abstract class BaseEntity : Component, IEntity
    {
        public void Recycle() => this.Manager.Recycle(this);
    }

    public abstract class Entity : BaseEntity, IEntityWithoutParams
    {
    }

    public abstract class Entity<TParams> : BaseEntity, IEntityWithParams<TParams>
    {
        TParams IEntityWithParams<TParams>.Params { get => this.Params; set => this.Params = value; }

        public TParams Params { get; private set; } = default!;
    }
}