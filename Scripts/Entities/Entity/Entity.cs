namespace UniT.Entities
{
    public abstract class BaseEntity : Component, IEntity
    {
        bool IEntity.IsDestroyed => !this;

        public void Recycle() => this.Manager.Recycle(this);
    }

    public abstract class Entity : BaseEntity, IEntityWithoutModel
    {
    }

    public abstract class Entity<TModel> : BaseEntity, IEntityWithModel<TModel>
    {
        TModel IEntityWithModel<TModel>.Model { set => this.Model = value; }

        public TModel Model { get; private set; }
    }
}