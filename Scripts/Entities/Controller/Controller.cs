namespace UniT.Entities.Controller
{
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    public abstract class Controller<TOwner> : IController where TOwner : IComponent, IHasController
    {
        IHasController IController.Owner { set => this.Owner = (TOwner)value; }

        void IController.OnInstantiate() => this.OnInstantiate();

        void IController.OnSpawn() => this.OnSpawn();

        void IController.OnRecycle() => this.OnRecycle();

        protected TOwner Owner { get; private set; }

        protected IEntityManager Manager => this.Owner.Manager;

        protected Transform Transform => this.Owner.Transform;

        protected virtual void OnInstantiate() { }

        protected virtual void OnSpawn() { }

        protected virtual void OnRecycle() { }

        #if UNIT_UNITASK
        protected CancellationToken GetCancellationTokenOnRecycle() => this.Owner.GetCancellationTokenOnRecycle();
        #endif
    }
}