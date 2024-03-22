namespace UniT.ECC.Entity.Controller
{
    using UniT.ECC.Component.Controller;
    using UniT.ECC.Controller;

    public abstract class BaseEntityController<TEntity> : ComponentController<TEntity>, IEntityController where TEntity : IEntity, IHasController
    {
        protected TEntity Entity => this.Owner;

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