namespace UniT.Entities.Controller
{
    public abstract class Controller<TEntity> : IController where TEntity : IEntity, IHasController
    {
        IHasController IController.Owner { set => this.Entity = (TEntity)value; }

        protected TEntity Entity { get; private set; }
    }
}