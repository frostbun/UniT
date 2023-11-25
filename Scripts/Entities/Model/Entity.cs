namespace UniT.Entities.Model
{
    public abstract class Entity<TModel> : BaseEntity, IEntityWithModel<TModel>
    {
        TModel IEntityWithModel<TModel>.Model { set => this.Model = value; }

        protected TModel Model { get; private set; }
    }
}