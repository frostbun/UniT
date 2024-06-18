#nullable enable
namespace UniT.Entities.Component
{
    using UniT.Entities.Entity;
    using UniT.Extensions;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    public abstract class Component : BetterMonoBehavior, IComponent
    {
        IEntityManager IComponent.Manager { get => this.Manager; set => this.Manager = value; }

        IEntity IComponent.Entity { get => this.Entity; set => this.Entity = value; }

        public IEntityManager Manager { get; private set; } = null!;

        public IEntity Entity { get; private set; } = null!;

        public string Name => this.gameObject.name;

        public GameObject GameObject => this.gameObject;

        public Transform Transform { get; private set; } = null!;

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
        private CancellationTokenSource? recycleCts;

        public CancellationToken GetCancellationTokenOnRecycle()
        {
            return (this.recycleCts ??= new CancellationTokenSource()).Token;
        }
        #endif

        protected virtual void OnInstantiate() { }

        protected virtual void OnSpawn() { }

        protected virtual void OnRecycle() { }
    }
}