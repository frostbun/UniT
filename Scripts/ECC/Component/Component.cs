namespace UniT.ECC.Component
{
    using UniT.Utilities;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    public abstract class Component : BetterMonoBehavior, IComponent
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
        private CancellationTokenSource recycleCts;

        public CancellationToken GetCancellationTokenOnRecycle()
        {
            return (this.recycleCts ??= new CancellationTokenSource()).Token;
        }
        #endif

        protected IEntityManager Manager { get; private set; }

        protected Transform Transform { get; private set; }

        protected virtual void OnInstantiate() { }

        protected virtual void OnSpawn() { }

        protected virtual void OnRecycle() { }
    }
}