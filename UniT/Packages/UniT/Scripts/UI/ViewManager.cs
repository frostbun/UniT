namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Extensions;
    using UniT.Extensions.UniTask;
    using UniT.Utilities;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class ViewManager : MonoBehaviour, IViewManager
    {
        private class ViewInstance : IViewManager.IViewInstance
        {
            public IViewManager Manager => this.manager;

            private ViewStatus _currentStatus = ViewStatus.Hidden;

            public ViewStatus CurrentStatus
            {
                get => this._currentStatus;
                private set
                {
                    this._currentStatus = value;
                    this.manager.Logger.Debug($"{this.view.GetType().Name} status: {value}");
                }
            }

            private readonly IView       view;
            private readonly IPresenter  presenter;
            private readonly ViewManager manager;
            public readonly  Transform   transform;

            public ViewInstance(IView view, IPresenter presenter, ViewManager manager)
            {
                this.view      = view;
                this.presenter = presenter;
                this.manager   = manager;
                this.transform = view.transform;

                this.view.Instance  = this;
                this.view.Presenter = this.presenter;
                this.presenter.View = this.view;

                this.view.Initialize();
                this.presenter.Initialize();

                this.manager.Logger.Debug($"Instantiated {this.view.GetType().Name}");
            }

            public IViewManager.IViewInstance BindModel(object model)
            {
                this.EnsureViewIsNotDisposed();
                this.presenter.Model = model;
                return this;
            }

            public void Stack()
            {
                this.Show_Internal(ViewStatus.Stacking);
                this.AddToStack();
            }

            public void Float()
            {
                this.Show_Internal(ViewStatus.Floating);
            }

            public void Detach()
            {
                this.Show_Internal(ViewStatus.Detached);
            }

            public void Hide()
            {
                this.EnsureViewIsHidden();
                this.RemoveFromStack();
            }

            public void Dispose()
            {
                this.EnsureViewIsHidden();

                this.manager.instances.Remove(this.view.GetType());
                this.RemoveFromStack();

                this.CurrentStatus = ViewStatus.Disposed;
                this.presenter.Dispose();
                this.view.Dispose();
                Destroy(this.view.gameObject);

                if (this.manager.keys.Remove(this.view.GetType(), out var key))
                {
                    this.manager.addressableManager.Unload(key);
                }
            }

            private void Show_Internal(ViewStatus status)
            {
                this.EnsureViewIsHidden();
                this.transform.SetParent(
                    status switch
                    {
                        ViewStatus.Stacking => this.manager.stackingViewsContainer,
                        ViewStatus.Floating => this.manager.floatingViewsContainer,
                        ViewStatus.Detached => this.manager.detachedViewsContainer,
                        _                   => throw new ArgumentOutOfRangeException(nameof(status), status, null),
                    },
                    false
                );
                this.transform.SetAsLastSibling();
                this.CurrentStatus = status;
                this.view.OnShow();
            }

            private void AddToStack()
            {
                var index = this.manager.instanceStack.IndexOf(this);
                if (index == -1)
                {
                    this.manager.instanceStack.Add(this);
                }
                else
                {
                    this.manager.instanceStack.RemoveRange(index, this.manager.instanceStack.Count - index - 1);
                }
                this.manager.instances.Values.ToArray()
                    .Where(instance => instance != this && instance.CurrentStatus is ViewStatus.Floating or ViewStatus.Stacking)
                    .ForEach(instance => instance.EnsureViewIsHidden());
            }

            private void RemoveFromStack()
            {
                this.manager.instanceStack.Remove(this);
                if (this.manager.StackingView is not null || this.manager.instanceStack.Count < 1) return;
                this.manager.instanceStack[^1].Stack();
            }

            private void EnsureViewIsHidden()
            {
                this.EnsureViewIsNotDisposed();
                if (this._currentStatus is ViewStatus.Hidden) return;
                this.transform.SetParent(this.manager.hiddenViewsContainer, false);
                this.CurrentStatus = ViewStatus.Hidden;
                this.view.OnHide();
            }

            private void EnsureViewIsNotDisposed()
            {
                if (this.CurrentStatus is ViewStatus.Disposed) throw new ObjectDisposedException(this.view.GetType().Name);
            }
        }

        [SerializeField]
        private RectTransform hiddenViewsContainer;

        [SerializeField]
        private RectTransform stackingViewsContainer;

        [SerializeField]
        private RectTransform floatingViewsContainer;

        [SerializeField]
        private RectTransform detachedViewsContainer;

        public ILogger Logger { get; private set; }

        private          IAddressableManager            addressableManager;
        private          IPresenterFactory              presenterFactory;
        private readonly Dictionary<Type, ViewInstance> instances     = new();
        private readonly Dictionary<Type, string>       keys          = new();
        private readonly List<ViewInstance>             instanceStack = new();

        public void Construct(IAddressableManager addressableManager, IPresenterFactory presenterFactory = null, ILogger logger = null)
        {
            this.addressableManager = addressableManager;
            this.presenterFactory   = presenterFactory ?? IPresenterFactory.Factory.Create(type => (IPresenter)Activator.CreateInstance(type));
            this.Logger             = logger ?? ILogger.Factory.CreateDefault(this.GetType().Name);
            DontDestroyOnLoad(this);
        }

        public IViewManager.IViewInstance StackingView => this.instanceStack.LastOrDefault(instance => instance.CurrentStatus is ViewStatus.Stacking);

        public IEnumerable<IViewManager.IViewInstance> FloatingViews => this.instances.Values.Where(instance => instance.CurrentStatus is ViewStatus.Floating).OrderByDescending(instance => instance.transform.GetSiblingIndex());

        public IEnumerable<IViewManager.IViewInstance> DetachedViews => this.instances.Values.Where(instance => instance.CurrentStatus is ViewStatus.Stacking).OrderByDescending(instance => instance.transform.GetSiblingIndex());

        public IViewManager.IViewInstance GetView<TView, TPresenter>(TView view)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.instances.GetOrAdd(
                typeof(TView),
                () => new(view, this.presenterFactory.Create(typeof(TPresenter)), this)
            );
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>(string key)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.instances.GetOrAdd(
                typeof(TView),
                () => this.addressableManager.LoadComponent<TView>(key).ContinueWith(view =>
                {
                    this.keys.Add(typeof(TView), key);
                    return new ViewInstance(Instantiate(view), this.presenterFactory.Create(typeof(TPresenter)), this);
                })
            ).Cast<ViewInstance, IViewManager.IViewInstance>();
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>()
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.GetView<TView, TPresenter>(typeof(TView).GetKeyAttribute());
        }
    }
}