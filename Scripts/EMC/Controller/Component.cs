namespace UniT.EMC.Controller
{
    using System;
    using UnityEngine;

    public abstract class Component<TController> : MonoBehaviour, IHasController where TController : IController
    {
        Type IHasController.ControllerType => this.ControllerType;

        IController IHasController.Controller { set => this.Controller = (TController)value; }

        protected virtual Type ControllerType { get; } = typeof(TController);

        protected TController Controller { get; private set; }
    }
}