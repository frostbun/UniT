#nullable enable
namespace UniT.Entities.Entity.Controller
{
    using UniT.Entities.Component.Controller;
    using UniT.Entities.Controller;

    public abstract class BaseEntityController<TEntity> : ComponentController<TEntity>, IEntityController where TEntity : IEntity, IHasController
    {
        protected new TEntity Entity => this.Owner;

        protected void Recycle() => this.Owner.Recycle();
    }

    public abstract class EntityController<TEntity> : BaseEntityController<TEntity> where TEntity : IEntityWithoutParams, IHasController
    {
    }

    public abstract class EntityController<TEntity, TParams> : BaseEntityController<TEntity> where TEntity : IEntityWithParams<TParams>, IHasController
    {
        protected TParams Params => this.Owner.Params;
    }
}