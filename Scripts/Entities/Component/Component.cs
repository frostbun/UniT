namespace UniT.Entities
{
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    public abstract class Component : UnmanagedComponent, IComponent
    {
        IEntityManager IComponent.Manager { get => this.Manager; set => this.Manager = value; }

        Transform IComponent.Transform => this.Transform;

        void IComponent.OnInstantiate()
        {
            this.Transform = this.transform;
            this.OnInstantiate();
        }

        void IComponent.OnSpawn()
        {
            this.OnSpawn();
        }

        void IComponent.OnRecycle()
        {
            #if UNIT_UNITASK
            this.recycleCts?.Cancel();
            this.recycleCts?.Dispose();
            this.recycleCts = null;
            #endif
            this.OnRecycle();
        }

        #if UNIT_UNITASK
        CancellationToken IComponent.GetCancellationTokenOnRecycle() => this.GetCancellationTokenOnRecycle();
        #endif

        protected IEntityManager Manager { get; private set; }

        protected Transform Transform { get; private set; }

        protected virtual void OnInstantiate() { }

        protected virtual void OnSpawn() { }

        protected virtual void OnRecycle() { }

        #if UNIT_UNITASK
        private CancellationTokenSource recycleCts;

        protected CancellationToken GetCancellationTokenOnRecycle()
        {
            return (this.recycleCts ??= new CancellationTokenSource()).Token;
        }
        #endif
    }
}