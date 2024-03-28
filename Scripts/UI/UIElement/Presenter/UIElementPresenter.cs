namespace UniT.UI.UIElement.Presenter
{
    using UniT.UI.Presenter;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #else
    using System.Collections;
    using System.Collections.Generic;
    #endif

    public abstract class BaseUIElementPresenter<TUIElement> : Presenter<TUIElement> where TUIElement : IUIElement, IHasPresenter
    {
        protected TUIElement UIElement => this.Owner;

        protected IUIManager Manager => this.Owner.Manager;

        protected Transform Transform => this.Owner.Transform;

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

        public virtual void OnDispose() { }
    }

    public abstract class UIElementPresenter<TUIElement> : BaseUIElementPresenter<TUIElement> where TUIElement : IUIElementWithoutParams, IHasPresenter
    {
    }

    public abstract class UIElementPresenter<TUIElement, TParams> : BaseUIElementPresenter<TUIElement> where TUIElement : IUIElementWithParams<TParams>, IHasPresenter
    {
        protected TParams Params => this.Owner.Params;
    }
}