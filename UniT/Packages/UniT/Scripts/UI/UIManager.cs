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

        public void Stack(IView view, bool force = false) => this.Show_Internal(view, force, IView.Status.Stacking);

        public void Float(IView view, bool force = false) => this.Show_Internal(view, force, IView.Status.Floating);

        public void Dock(IView view, bool force = false) => this.Show_Internal(view, force, IView.Status.Docked);

        public void Hide(IView view, bool removeFromStack = true, bool autoStack = true)
        {
            if (view.CurrentStatus is IView.Status.Hidden) return;
            view.transform.SetParent(this.hiddenViews, false);
            this.Logger.Debug($"{view.GetType().Name} status: {view.CurrentStatus = IView.Status.Hidden}");
            view.OnHide();
            if (removeFromStack) this.RemoveFromStack(view);
            if (autoStack) this.StackNextView();
        }

        public void Dispose(IView view, bool autoStack = true)
        {
            this.Hide(view, true, autoStack);
            Destroy(this.gameObject);
            this.views.Remove(view.GetType());
            if (!this.keys.Remove(view.GetType(), out var key)) return;
            this.assetsManager.Unload(key);
            this.Logger.Debug($"{view.GetType().Name} status: {view.CurrentStatus = IView.Status.Disposed}");
            view.OnDispose();
        }

        private IView Initialize_Internal(IView view)
        {
            if (view is IViewWithPresenter viewWithPresenter)
            {
                var presenter = this.presenterFactory.Create(viewWithPresenter.PresenterType);
                presenter.View              = view;
                viewWithPresenter.Presenter = presenter;
            }
            view.Manager = this;
            view.transform.SetParent(this.hiddenViews, false);
            view.CurrentStatus = IView.Status.Hidden;
            this.Logger.Debug($"Initialized {view.GetType().Name}");
            view.OnInitialize();
            return view;
        }

        private void Show_Internal(IView view, bool force, IView.Status nextStatus)
        {
            if (!force && view.CurrentStatus == nextStatus) return;
            this.Hide(view, false, false);
            switch (nextStatus)
            {
                case IView.Status.Stacking:
                {
                    this.AddToStack(view);
                    this.HideUndockedViews();
                    view.transform.SetParent(this.stackingViews, false);
                    break;
                }
                case IView.Status.Floating:
                {
                    view.transform.SetParent(this.floatingViews, false);
                    break;
                }
                case IView.Status.Docked:
                {
                    view.transform.SetParent(this.dockedViews, false);
                    break;
                }
            }
            view.transform.SetAsLastSibling();
            this.Logger.Debug($"{view.GetType().Name} status: {view.CurrentStatus = nextStatus}");
            view.OnShow();
        }

        private void AddToStack(IView view)
        {
            var index = this.stack.IndexOf(view);
            if (index == -1)
            {
                this.stack.Add(view);
            }
            else
            {
                this.stack.RemoveRange(index + 1, this.stack.Count - index - 1);
            }
        }

        private void RemoveFromStack(IView view)
        {
            this.stack.Remove(view);
        }

        private void StackNextView()
        {
            if (this.StackingView is not null) return;
            this.NextStackingView?.Stack();
        }

        private void HideUndockedViews()
        {
            this.views.Values.ToArray()
                .Where(view => view.CurrentStatus is IView.Status.Floating or IView.Status.Stacking)
                .ForEach(view => this.Hide(view, false, false));
        }

        public IItemAdapter Initialize(IItemAdapter itemAdapter)
        {
            itemAdapter.Initialize(this, this.itemPresenterFactory);
            return itemAdapter;
        }
    }
}