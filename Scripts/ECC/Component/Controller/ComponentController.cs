namespace UniT.ECC.Component.Controller
{
    using UniT.ECC.Controller;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    public abstract class ComponentController<TComponent> : Controller<TComponent>, IComponentController where TComponent : IComponent, IHasController
    {
        void IComponentController.OnInstantiate() => this.OnInstantiate();

        void IComponentController.OnSpawn() => this.OnSpawn();

        void IComponentController.OnRecycle() => this.OnRecycle();

        protected virtual void OnInstantiate() { }

        protected virtual void OnSpawn() { }

        protected virtual void OnRecycle() { }

        protected TComponent Component => this.Owner;

        protected IEntityManager Manager => this.Owner.Manager;

        protected Transform Transform => this.Owner.Transform;

        #if UNIT_UNITASK
        protected CancellationToken GetCancellationTokenOnRecycle() => this.Owner.GetCancellationTokenOnRecycle();
        #endif
    }
}