namespace UniT.ECC.Controller
{
    public abstract class Controller<T> : IController where T : IHasController
    {
        IHasController IController.Owner { set => this.Owner = (T)value; }

        protected T Owner { get; private set; }
    }
}