namespace UniT.Entities.Controller
{
    using System;

    public abstract class Entity<TController> : Entity, IEntityWithController where TController : IController
    {
        Type IEntityWithController.ControllerType => this.ControllerType;

        IController IEntityWithController.Controller { set => this.Controller = (TController)value; }

        protected virtual Type ControllerType => typeof(TController);

        protected TController Controller { get; private set; }
    }

    public abstract class Entity<TModel, TController> : Entities.Entity<TModel>, IEntityWithController where TController : IController
    {
        Type IEntityWithController.ControllerType => this.ControllerType;

        IController IEntityWithController.Controller { set => this.Controller = (TController)value; }

        protected virtual Type ControllerType => typeof(TController);

        protected TController Controller { get; private set; }
    }
}