namespace UniT.Entities.Controller
{
    using System;

    public abstract class Entity<TController> : Entity, IEntityWithController where TController : IController
    {
        Type IHasController.ControllerType => this.ControllerType;

        IController IHasController.Controller { set => this.Controller = (TController)value; }

        protected virtual Type ControllerType { get; } = typeof(TController);

        protected TController Controller { get; private set; }
    }

    public abstract class Entity<TModel, TController> : Model.Entity<TModel>, IEntityWithController where TController : IController
    {
        Type IHasController.ControllerType => this.ControllerType;

        IController IHasController.Controller { set => this.Controller = (TController)value; }

        protected virtual Type ControllerType { get; } = typeof(TController);

        protected TController Controller { get; private set; }
    }
}