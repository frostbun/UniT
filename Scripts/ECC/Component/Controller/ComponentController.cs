namespace UniT.ECC.Component.Controller
{
    using UniT.ECC.Controller;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #else
    using System.Collections;
    using System.Collections.Generic;
    #endif

    public abstract class ComponentController<TComponent> : Controller<TComponent>, IComponentController where TComponent : IComponent, IHasController
    {
        protected TComponent Component => this.Owner;

        protected IEntityManager Manager => this.Owner.Manager;

        protected Transform Transform => this.Owner.Transform;

        #if UNIT_UNITASK
        protected CancellationToken GetCancellationTokenOnRecycle() => this.Owner.GetCancellationTokenOnRecycle();
        #else
        protected void StartCoroutine(IEnumerator coroutine) => this.Owner.StartCoroutine(coroutine);

        protected void StopCoroutine(IEnumerator coroutine) => this.Owner.StopCoroutine(coroutine);

        protected IEnumerator GatherCoroutines(params IEnumerator[] coroutines) => this.Owner.GatherCoroutines(coroutines);

        protected IEnumerator GatherCoroutines(IEnumerable<IEnumerator> coroutines) => this.Owner.GatherCoroutines(coroutines);
        #endif

        public virtual void OnInstantiate() { }

        public virtual void OnSpawn() { }

        public virtual void OnRecycle() { }
    }
}