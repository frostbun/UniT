namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Extensions;
    using UniT.Extensions.UniTask;
    using UniT.UI.Interfaces;
    using UniT.UI.Item.Interfaces;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class UIManager : MonoBehaviour, IUIManager
    {
        [SerializeField]
        private RectTransform hiddenViews;

        [SerializeField]
        private RectTransform stackingViews;

        [SerializeField]
        private RectTransform floatingViews;

        [SerializeField]
        private RectTransform dockedViews;

        private          IPresenter.Factory       presenterFactory;
        private          IItemPresenter.Factory   itemPresenterFactory;
        private          IAssetsManager           assetsManager;
        private readonly Dictionary<Type, IView>  views = new();
        private readonly List<IView>              stack = new();
        private readonly Dictionary<Type, string> keys  = new();

        public UIManager Construct(
            IPresenter.Factory presenterFactory = null,
            IItemPresenter.Factory itemPresenterFactory = null,
            IAssetsManager assetsManager = null,
            ILogger logger = null
        )
        {
            this.presenterFactory     = presenterFactory ?? IPresenter.Factory.Default();
            this.itemPresenterFactory = itemPresenterFactory ?? IItemPresenter.Factory.Default();
            this.assetsManager        = assetsManager ?? IAssetsManager.Default();
            this.Logger               = logger ?? ILogger.Default(this.GetType().Name);
            return this.DontDestroyOnLoad();
        }

        #region Public APIs

        public ILogger Logger { get; private set; }

        public IView StackingView => this.stack.LastOrDefault(view => view.CurrentStatus is IView.Status.Stacking);

        public IView NextStackingView => this.stack.LastOrDefault(view => view.CurrentStatus is not IView.Status.Stacking);

        public IEnumerable<IView> FloatingViews => this.views.Values.Where(view => view.CurrentStatus is IView.Status.Floating);

        public IEnumerable<IView> DockedViews => this.views.Values.Where(view => view.CurrentStatus is IView.Status.Docked);

        public UniTask<IView> GetView<TView>(string key) where TView : Component, IView
        {
            return this.views.GetOrAdd(
                typeof(TView),
                () => this.assetsManager.LoadComponent<TView>(key).ContinueWith(viewPrefab =>
                {
                    this.keys.Add(typeof(TView), key);
                    return this.Initialize_Internal(Instantiate(viewPrefab));
                })
            );
        }

        public UniTask<IView> GetView<TView>() where TView : Component, IView
        {
            return this.GetView<TView>(typeof(TView).GetKey());
        }

        public IView Initialize(IView view)
        {
            return this.views.GetOrAdd(view.GetType(), () => this.Initialize_Internal(view));
        }

        public IItemAdapter Initialize(IItemAdapter itemAdapter)
        {
            itemAdapter.Initialize(this, this.itemPresenterFactory);
            return itemAdapter;
        }

        #endregion

        #region Internal APIs

        private IView Initialize_Internal(IView view)
        {
            if (view is IViewWithPresenter viewWithPresenter)
            {
                var presenter = this.presenterFactory.Create(viewWithPresenter.PresenterType);
                presenter.View              = view;
                viewWithPresenter.Presenter = presenter;
            }
            view.Initialize(this);
            return view;
        }

        void IUIManager.Stack(IView view)
        {
            view.transform.SetParent(this.stackingViews, false);
            view.transform.SetAsLastSibling();
            var index = this.stack.IndexOf(view);
            if (index == -1)
            {
                this.stack.Add(view);
            }
            else
            {
                this.stack.RemoveRange(index + 1, this.stack.Count - index - 1);
            }
            this.views.Values.ToArray()
                .Where(view => view.CurrentStatus is IView.Status.Floating or IView.Status.Stacking)
                .ForEach(view => view.Hide(false, false));
        }

        void IUIManager.Float(IView view)
        {
            view.transform.SetParent(this.floatingViews, false);
            view.transform.SetAsLastSibling();
        }

        void IUIManager.Dock(IView view)
        {
            view.transform.SetParent(this.dockedViews, false);
            view.transform.SetAsLastSibling();
        }

        void IUIManager.Hide(IView view)
        {
            view.transform.SetParent(this.hiddenViews, false);
        }

        void IUIManager.Dispose(IView view)
        {
            this.views.Remove(view.GetType());
            if (!this.keys.Remove(view.GetType(), out var key)) return;
            this.assetsManager.Unload(key);
        }

        void IUIManager.RemoveFromStack(IView view)
        {
            this.stack.Remove(view);
        }

        void IUIManager.StackNextView()
        {
            if (this.StackingView is not null) return;
            this.NextStackingView?.Stack();
        }

        #endregion
    }
}