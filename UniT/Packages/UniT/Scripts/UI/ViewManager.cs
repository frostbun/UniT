namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Extensions;
    using UniT.Extensions.UniTask;
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

                this.view.transform.SetParent(this.manager.canvas, false);
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
                this.Show_Internal();

                this.manager.instances.Values
                    .Where(instance => instance.CurrentStatus is ViewStatus.Floating or ViewStatus.Stacking)
                    .ForEach(instance => instance.Hide_Internal());

                this.manager.instanceStack.Remove(this);
                this.manager.instanceStack.Add(this);
                this.CurrentStatus = ViewStatus.Stacking;
            }

            public void Float()
            {
                this.Show_Internal();
                this.CurrentStatus = ViewStatus.Floating;
            }

            public void Detach()
            {
                this.Show_Internal();
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

                this.view.Dispose();
                this.presenter.Dispose();

                this.manager.instances.Remove(this.view.GetType());
                this.RemoveFromStack();

                Destroy(this.view.gameObject);

                if (this.manager.keys.Remove(this.view.GetType(), out var key))
                {
                    this.manager.addressableManager.Unload(key);
                }

                this.CurrentStatus = ViewStatus.Disposed;
            }

            private void Show_Internal()
            {
                this.EnsureViewIsHidden();
                this.view.OnShow();
                this.view.gameObject.SetActive(true);
            }

            private void Hide_Internal()
            {
                this.EnsureViewIsNotDisposed();
                if (this._currentStatus is ViewStatus.Hidden) return;
                this.view.gameObject.SetActive(false);
                this.view.OnHide();
                this.CurrentStatus = ViewStatus.Hidden;
            }

            private void RemoveFromStack()
            {
                this.manager.instanceStack.Remove(this);
                if (this.manager.CurrentView != null || this.manager.instanceStack.Count < 1) return;
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
        private RectTransform canvas;

        private          IAddressableManager            addressableManager;
        private          ILogger                        logger;
        private readonly Dictionary<Type, ViewInstance> instances     = new();
        private readonly Dictionary<Type, string>       keys          = new();
        private readonly List<ViewInstance>             instanceStack = new();

        public void Inject(IAddressableManager addressableManager, ILogger logger = null)
        {
            this.addressableManager = addressableManager;
            this.logger             = logger;
        }

        public IViewManager.IViewInstance CurrentView => this.instances.Values.SingleOrDefault(instance => instance.CurrentStatus is ViewStatus.Stacking);

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
            ).Cast<ViewInstance, IViewManager.IViewInstance>();
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