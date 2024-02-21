namespace UniT.Entities
{
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #else
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    #endif

    public abstract class Component : UnmanagedComponent, IComponent
    {
        IEntityManager IComponent.Manager { get => this.Manager; set => this.Manager = value; }

        void IComponent.OnInstantiate()
        {
            this.transform = base.transform;
            this.OnInstantiate();
        }

        void IComponent.OnSpawn()
        {
            this.OnSpawn();
        }

        void IComponent.OnRecycle()
        {
            #if UNIT_UNITASK
            this.hideCts?.Cancel();
            this.hideCts?.Dispose();
            this.hideCts = null;
            #endif
            this.OnRecycle();
        }

        protected IEntityManager Manager { get; private set; }

        public new Transform transform { get; private set; }

        #region Helpers

        #if UNIT_UNITASK
        private CancellationTokenSource hideCts;

        protected CancellationToken GetCancellationTokenOnHide()
        {
            return (this.hideCts ??= new CancellationTokenSource()).Token;
        }
        #endif

        #endregion

        protected virtual void OnInstantiate()
        {
        }

        protected virtual void OnSpawn()
        {
        }

        protected virtual void OnRecycle()
        {
        }
    }
}