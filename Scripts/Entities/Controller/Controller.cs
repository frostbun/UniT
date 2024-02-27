namespace UniT.Entities.Controller
{
    public abstract class Controller<TOwner> : IController where TOwner : IComponent, IHasController
    {
        IHasController IController.Owner { set => this.Owner = (TOwner)value; }

        protected TOwner Owner { get; private set; }

        protected IEntityManager Manager => this.Owner.Manager;
    }
}