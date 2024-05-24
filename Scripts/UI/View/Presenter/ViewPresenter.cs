#nullable enable
namespace UniT.UI.View.Presenter
{
    using UniT.UI.Activity;
    using UniT.UI.Presenter;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #else
    using System.Collections;
    using System.Collections.Generic;
    #endif

    public abstract class BaseViewPresenter<TView> : Presenter<TView>, IViewPresenter where TView : IView, IHasPresenter
    {
        protected TView View => this.Owner;

        protected IUIManager Manager => this.Owner.Manager;

        protected IActivity Activity => this.Owner.Activity;

        #if UNIT_UNITASK
        protected CancellationToken GetCancellationTokenOnRecycle() => this.Owner.GetCancellationTokenOnHide();
        #else
        protected void StartCoroutine(IEnumerator coroutine) => this.Owner.StartCoroutine(coroutine);

        protected void StopCoroutine(IEnumerator coroutine) => this.Owner.StopCoroutine(coroutine);

        protected IEnumerator GatherCoroutines(params IEnumerator[] coroutines) => this.Owner.GatherCoroutines(coroutines);

        protected IEnumerator GatherCoroutines(IEnumerable<IEnumerator> coroutines) => this.Owner.GatherCoroutines(coroutines);
        #endif

        public virtual void OnInitialize() { }

        public virtual void OnShow() { }

        public virtual void OnHide() { }
    }

    public abstract class ViewPresenter<TView> : BaseViewPresenter<TView> where TView : IViewWithoutParams, IHasPresenter
    {
    }

    public abstract class ViewPresenter<TView, TParams> : BaseViewPresenter<TView> where TView : IViewWithParams<TParams>, IHasPresenter
    {
        protected TParams Params => this.Owner.Params;
    }
}