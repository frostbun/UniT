namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Extensions;
    using UniT.Extensions.UniTask;
    using UniT.Utils;
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

                if (this.view is IInitializable initializableView) initializableView.Initialize();
                if (this.presenter is IInitializable initializablePresenter) initializablePresenter.Initialize();

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
                this.EnsureViewIsHidden();
                this.view.OnShow();
                this.transform.SetParent(this.manager.stackingViewsContainer, false);
                this.transform.SetAsLastSibling();
                this.manager.instances.Values
                    .Where(instance => instance.CurrentStatus is ViewStatus.Floating or ViewStatus.Stacking)
                    .ForEach(instance => instance.Hide_Internal());
                this.manager.instanceStack.Remove(this);
                this.manager.instanceStack.Add(this);
                this.CurrentStatus = ViewStatus.Stacking;
            }

            public void Float()
            {
                this.EnsureViewIsHidden();
                this.view.OnShow();
                this.transform.SetParent(this.manager.floatingViewsContainer, false);
                this.transform.SetAsLastSibling();
                this.CurrentStatus = ViewStatus.Floating;
            }

            public void Detach()
            {
                this.EnsureViewIsHidden();
                this.view.OnShow();
                this.transform.SetParent(this.manager.detachedViewsContainer, false);
                this.transform.SetAsLastSibling();
                this.CurrentStatus = ViewStatus.Detached;
            }

            public void Hide()
            {
                this.Hide_Internal();
                this.RemoveFromStack();
            }

            public void Dispose()
            {
                this.EnsureViewIsNotDisposed();
                this.EnsureViewIsHidden();

                if (this.view is IDisposable disposableView) disposableView.Dispose();
                if (this.presenter is IDisposable disposablePresenter) disposablePresenter.Dispose();

                this.manager.instances.Remove(this.view.GetType());
                this.RemoveFromStack();

                Destroy(this.view.gameObject);

                if (this.manager.keys.Remove(this.view.GetType(), out var key))
                {
                    this.manager.addressableManager.Unload(key);
                }

                this.CurrentStatus = ViewStatus.Disposed;
            }

            private void Hide_Internal()
            {
                this.EnsureViewIsNotDisposed();
                if (this._currentStatus is ViewStatus.Hidden) return;
                this.transform.SetParent(this.manager.hiddenViewsContainer, false);
                this.view.OnHide();
                this.CurrentStatus = ViewStatus.Hidden;
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
                if (this._currentStatus is not ViewStatus.Hidden) this.Hide_Internal();
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

        private          IPresenterFactory              presenterFactory;
        private          IAddressableManager            addressableManager;
        private readonly Dictionary<Type, ViewInstance> instances     = new();
        private readonly Dictionary<Type, string>       keys          = new();
        private readonly List<ViewInstance>             instanceStack = new();

        public void Inject(IPresenterFactory presenterFactory, IAddressableManager addressableManager, ILogger logger)
        {
            this.presenterFactory   = presenterFactory;
            this.addressableManager = addressableManager;
            this.Logger             = logger;
            this.Logger.Info($"{nameof(ViewManager)} instantiated", Color.green);
        }

        public IViewManager.IViewInstance StackingView => this.instances.Values.SingleOrDefault(instance => instance.CurrentStatus is ViewStatus.Stacking);

        public IEnumerable<IViewManager.IViewInstance> FloatingViews => this.instances.Values.Where(instance => instance.CurrentStatus is ViewStatus.Floating).OrderByDescending(instance => instance.transform.GetSiblingIndex());

        public IEnumerable<IViewManager.IViewInstance> DetachedViews => this.instances.Values.Where(instance => instance.CurrentStatus is ViewStatus.Stacking).OrderByDescending(instance => instance.transform.GetSiblingIndex());

        public IViewManager.IViewInstance GetView<TView, TPresenter>(TView view)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.instances.GetOrAdd(
                typeof(TView),
                () => new(
                    view,
                    this.presenterFactory.Create(typeof(TPresenter)),
                    this
                )
            );
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>(string key)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.instances.GetOrAdd(
                typeof(TView),
                () => this.addressableManager.LoadComponent<TView>(key)
                          .ContinueWith(view =>
                          {
                              this.keys.Add(typeof(TView), key);
                              return new ViewInstance(
                                  Instantiate(view),
                                  this.presenterFactory.Create(typeof(TPresenter)),
                                  this
                              );
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