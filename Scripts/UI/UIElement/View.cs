namespace UniT.UI.View
{
    using UniT.UI.Activity;
    using UniT.Utilities;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    [RequireComponent(typeof(RectTransform))]
    public abstract class BaseView : BetterMonoBehavior, IView
    {
        IUIManager IView.Manager { get => this.Manager; set => this.Manager = value; }

        IActivity IView.Activity { get => this.Activity; set => this.Activity = value; }

        public IUIManager Manager { get; private set; }

        public IActivity Activity { get; private set; }

        public string Name => this.name;

        public GameObject GameObject => this.gameObject;

        public RectTransform Transform { get; private set; }

        void IView.OnInitialize()
        {
            this.Transform = (RectTransform)this.transform;
            this.OnInitialize();
        }

        void IView.OnShow()
        {
            this.OnShow();
        }

        void IView.OnHide()
        {
            #if UNIT_UNITASK
            this.hideCts?.Cancel();
            this.hideCts?.Dispose();
            this.hideCts = null;
            #endif
            this.OnHide();
        }

        void IView.OnDispose()
        {
            this.OnDispose();
        }

        #if UNIT_UNITASK
        protected internal CancellationTokenSource hideCts;

        public CancellationToken GetCancellationTokenOnHide()
        {
            return (this.hideCts ??= new CancellationTokenSource()).Token;
        }
        #endif

        protected virtual void OnInitialize() { }

        protected virtual void OnShow() { }

        protected virtual void OnHide() { }

        protected virtual void OnDispose() { }
    }

    public abstract class View : BaseView, IViewWithoutParams
    {
    }

    public abstract class View<TParams> : BaseView, IViewWithParams<TParams>
    {
        TParams IViewWithParams<TParams>.Params { get => this.Params; set => this.Params = value; }

        public TParams Params { get; private set; }
    }
}