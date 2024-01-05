namespace UniT.Entities.Controller
{
    public abstract class Controller<TEntity> : IController where TEntity : IEntityWithController
    {
        IEntityWithController IController.Owner { set => this.Entity = (TEntity)value; }

        protected TEntity Entity { get; private set; }
    }
}