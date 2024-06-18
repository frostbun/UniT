#nullable enable
namespace UniT.UI.View
{
    using UniT.Extensions;
    using UniT.UI.Activity;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    [RequireComponent(typeof(RectTransform))]
    public abstract class BaseView : BetterMonoBehavior, IView
    {
        IUIManager IView.Manager { get => this.Manager; set => this.Manager = value; }

        IActivity IView.Activity { get => this.Activity; set => this.Activity = value; }

        public IUIManager Manager { get; private set; } = null!;

        public IActivity Activity { get; private set; } = null!;

        public string Name => this.gameObject.name;

        public GameObject GameObject => this.gameObject;

        public RectTransform Transform { get; private set; } = null!;

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

        #if UNIT_UNITASK
        private CancellationTokenSource? hideCts;

        public CancellationToken GetCancellationTokenOnHide()
        {
            return (this.hideCts ??= new CancellationTokenSource()).Token;
        }
        #endif

        protected virtual void OnInitialize() { }

        protected virtual void OnShow() { }

        protected virtual void OnHide() { }
    }

    public abstract class View : BaseView, IViewWithoutParams
    {
    }

    public abstract class View<TParams> : BaseView, IViewWithParams<TParams>
    {
        TParams IViewWithParams<TParams>.Params { get => this.Params; set => this.Params = value; }

        public TParams Params { get; private set; } = default!;
    }
}