namespace UniT.EMC.Controller
{
    public abstract class Controller<TComponent> : IController where TComponent : IHasController
    {
        IHasController IController.Component { set => this.Component = (TComponent)value; }

        protected TComponent Component { get; private set; }
    }
}