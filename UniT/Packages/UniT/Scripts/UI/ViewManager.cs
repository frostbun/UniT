namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Extensions;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class ViewManager : MonoBehaviour, IViewManager
    {
        private class ViewInstance : IViewManager.IViewInstance
        {
            private ViewStatus _currentStatus = ViewStatus.Hidden;

            public ViewStatus CurrentStatus
            {
                get => this._currentStatus;
                private set
                {
                    this._currentStatus = value;
                    this.manager.logger?.Debug($"{this.view.GetType().Name} status: {value}");
                }
            }

            private readonly IView       view;
            private readonly IPresenter  presenter;
            private readonly ViewManager manager;

            public ViewInstance(IView view, IPresenter presenter, ViewManager manager)
            {
                this.view      = view;
                this.presenter = presenter;
                this.manager   = manager;

                this.view.Instance  = this;
                this.view.Presenter = this.presenter;
                this.presenter.View = this.view;

                this.view.Initialize();
                this.presenter.Initialize();

                this.manager.logger?.Debug($"Instantiated {this.view.GetType().Name}");
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

                this.manager.instances.Values
                    .Where(instance => instance.CurrentStatus is ViewStatus.Floating)
                    .ForEach(instance => instance.Hide());

                if (this.manager.CurrentView is ViewInstance currentInstance)
                {
                    currentInstance.view.OnHide();
                    currentInstance.CurrentStatus = ViewStatus.Hidden;
                }

                this.view.OnShow();
                this.view.transform.SetParent(this.manager.stackingViewsContainer, false);
                this.view.transform.SetAsLastSibling();
                this.CurrentStatus = ViewStatus.Stacking;
            }

            public void Float()
            {
                this.EnsureViewIsHidden();
                this.view.OnShow();
                this.view.transform.SetParent(this.manager.floatingViewsContainer, false);
                this.view.transform.SetAsLastSibling();
                this.CurrentStatus = ViewStatus.Floating;
            }

            public void Detach()
            {
                this.EnsureViewIsHidden();
                this.view.OnShow();
                this.view.transform.SetParent(this.manager.detachedViewsContainer, false);
                this.view.transform.SetAsLastSibling();
                this.CurrentStatus = ViewStatus.Detached;
            }

            public void Hide()
            {
                this.EnsureViewIsNotDisposed();
                if (this._currentStatus is ViewStatus.Hidden) return;
                this.view.transform.SetParent(this.manager.hiddenViewsContainer, false);
                this.view.OnHide();
                this.CurrentStatus = ViewStatus.Hidden;

                if (this.manager.CurrentView != null || this.manager.stackingViewsContainer.childCount <= 0) return;
                var nextView     = this.manager.stackingViewsContainer.GetChild(this.manager.stackingViewsContainer.childCount - 1);
                var nextInstance = this.manager.instances[nextView.GetType()];
                nextInstance.Stack();
            }

            public void Dispose()
            {
                this.EnsureViewIsNotDisposed();
                this.EnsureViewIsHidden();

                this.view.Dispose();
                this.presenter.Dispose();

                this.manager.instances.Remove(this.view.GetType());
                Destroy(this.view.gameObject);

                if (this.manager.keys.Remove(this.view.GetType(), out var key))
                {
                    this.manager.addressableManager.Unload(key);
                }

                this.CurrentStatus = ViewStatus.Disposed;
            }

            private void EnsureViewIsHidden()
            {
                this.EnsureViewIsNotDisposed();
                if (this._currentStatus is not ViewStatus.Hidden) this.Hide();
            }

            private void EnsureViewIsNotDisposed()
            {
                if (this.CurrentStatus is ViewStatus.Disposed) throw new ObjectDisposedException(this.view.GetType().Name);
            }
        }

        [SerializeField]
        private RectTransform stackingViewsContainer;

        [SerializeField]
        private RectTransform floatingViewsContainer;

        [SerializeField]
        private RectTransform detachedViewsContainer;

        [SerializeField]
        private RectTransform hiddenViewsContainer;

        private          IAddressableManager            addressableManager;
        private          ILogger                        logger;
        private readonly Dictionary<Type, ViewInstance> instances = new();
        private readonly Dictionary<Type, string>       keys      = new();

        public void Inject(IAddressableManager addressableManager, ILogger logger = null)
        {
            this.addressableManager = addressableManager;
            this.logger             = logger;
        }

        public IViewManager.IViewInstance CurrentView => this.instances.Values.FirstOrDefault(instance => instance.CurrentStatus is ViewStatus.Stacking);

        public IViewManager.IViewInstance GetView<TView, TPresenter>(TView view, Func<TPresenter> presenterFactory)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.instances.GetOrAdd(typeof(TView), () => new(view, presenterFactory(), this));
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>(string key, Func<TPresenter> presenterFactory)
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
                                  presenterFactory(),
                                  this
                              );
                          })
            ).ContinueWith(instance => (IViewManager.IViewInstance)instance);
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>(Func<TPresenter> presenterFactory)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.GetView<TView, TPresenter>(typeof(TView).GetKeyAttribute(), presenterFactory);
        }

        public IViewManager.IViewInstance GetView<TView, TPresenter>(TView view)
            where TView : Component, IView
            where TPresenter : IPresenter, new()
        {
            return this.GetView<TView, TPresenter>(view, () => new());
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>(string key)
            where TView : Component, IView
            where TPresenter : IPresenter, new()
        {
            return this.GetView<TView, TPresenter>(key, () => new());
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>()
            where TView : Component, IView
            where TPresenter : IPresenter, new()
        {
            return this.GetView<TView, TPresenter>(typeof(TView).GetKeyAttribute());
        }
    }
}