#nullable enable
namespace UniT.Entities.Component.Controller
{
    using System;
    using UniT.Entities.Controller;

    public abstract class Component<TController> : Component, IHasController where TController : IComponentController
    {
        Type IHasController.ControllerType => this.ControllerType;

        IController IHasController.Controller { set => this.Controller = (TController)value; }

        protected virtual Type ControllerType => typeof(TController);

        protected TController Controller { get; private set; } = default!;

        protected sealed override void OnInstantiate() => this.Controller.OnInstantiate();

        protected sealed override void OnSpawn() => this.Controller.OnSpawn();

        protected sealed override void OnRecycle() => this.Controller.OnRecycle();
    }
}