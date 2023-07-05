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
        private class Contract : IContract
        {
            private IContract.Status _currentStatus = IContract.Status.Hidden;

            public IContract.Status CurrentStatus
            {
                get => this._currentStatus;
                private set
                {
                    this._currentStatus = value;
                    this.manager.Logger.Debug($"{this.view.GetType().Name} status: {value}");
                }
            }

            public Transform Transform => this.view.Transform;

            private readonly IView                      view;
            private readonly IPresenter                 presenter;
            private readonly ViewManager                manager;
            private readonly Dictionary<string, object> extras;

            public Contract(IView view, IPresenter presenter, ViewManager manager)
            {
                this.view      = view;
                this.presenter = presenter;
                this.manager   = manager;
                this.extras    = new();

                this.view.Contract  = this.presenter.Contract = this;
                this.view.Presenter = this.presenter;
                this.presenter.View = this.view;

                this.view.Initialize();

                this.manager.Logger.Debug($"Instantiated {this.view.GetType().Name}");
            }

            public IContract PutExtra<T>(string key, T value)
            {
                this.extras[key] = value;
                return this;
            }

            public T GetExtra<T>(string key)
            {
                return (T)this.extras.GetOrDefault(key);
            }

            public void Stack()
            {
                this.Show_Internal(IContract.Status.Stacking);
                this.AddToStack();
            }

            public void Float()
            {
                this.Show_Internal(IContract.Status.Floating);
            }

            public void Detach()
            {
                this.Show_Internal(IContract.Status.Detached);
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

                this.CurrentStatus = IContract.Status.Disposed;
                this.view.Dispose();
                Destroy(this.view.GameObject);

                if (this.manager.keys.Remove(this.view.GetType(), out var key))
                {
                    this.manager.addressableManager.Unload(key);
                }
            }

            private void Show_Internal(IContract.Status status)
            {
                this.EnsureViewIsHidden();
                this.Transform.SetParent(
                    status switch
                    {
                        IContract.Status.Stacking => this.manager.stackingViewsContainer,
                        IContract.Status.Floating => this.manager.floatingViewsContainer,
                        IContract.Status.Detached => this.manager.detachedViewsContainer,
                        _                         => throw new ArgumentOutOfRangeException(nameof(status), status, null),
                    },
                    false
                );
                this.Transform.SetAsLastSibling();
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
                    .Where(instance => instance != this && instance.CurrentStatus is IContract.Status.Floating or IContract.Status.Stacking)
                    .ForEach(instance => instance.EnsureViewIsHidden());
            }

            private void RemoveFromStack()
            {
                this.manager.instanceStack.Remove(this);
                if (this.manager.StackingContract is not null || this.manager.instanceStack.Count < 1) return;
                this.manager.instanceStack[^1].Stack();
            }

            private void EnsureViewIsHidden()
            {
                this.EnsureViewIsNotDisposed();
                if (this._currentStatus is IContract.Status.Hidden) return;
                this.Transform.SetParent(this.manager.hiddenViewsContainer, false);
                this.CurrentStatus = IContract.Status.Hidden;
                this.view.OnHide();
            }

            private void EnsureViewIsNotDisposed()
            {
                if (this.CurrentStatus is IContract.Status.Disposed) throw new ObjectDisposedException(this.view.GetType().Name);
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

        private          IAddressableManager        addressableManager;
        private          IPresenterFactory          presenterFactory;
        private readonly Dictionary<Type, Contract> instances     = new();
        private readonly Dictionary<Type, string>   keys          = new();
        private readonly List<Contract>             instanceStack = new();

        public void Construct(IAddressableManager addressableManager, IPresenterFactory presenterFactory = null, ILogger logger = null)
        {
            this.addressableManager = addressableManager;
            this.presenterFactory   = presenterFactory ?? IPresenterFactory.Factory.Create(type => (IPresenter)Activator.CreateInstance(type));
            this.Logger             = logger ?? ILogger.Factory.CreateDefault(this.GetType().Name);
            DontDestroyOnLoad(this);
        }

        public IContract StackingContract => this.instanceStack.LastOrDefault(instance => instance.CurrentStatus is IContract.Status.Stacking);

        public IEnumerable<IContract> FloatingContracts => this.instances.Values.Where(instance => instance.CurrentStatus is IContract.Status.Floating).OrderByDescending(instance => instance.Transform.GetSiblingIndex());

        public IEnumerable<IContract> DetachedContracts => this.instances.Values.Where(instance => instance.CurrentStatus is IContract.Status.Stacking).OrderByDescending(instance => instance.Transform.GetSiblingIndex());

        public IContract GetContract<TView, TPresenter>(TView view)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.instances.GetOrAdd(
                typeof(TView),
                () => new(view, this.presenterFactory.Create(typeof(TPresenter)), this)
            );
        }

        public UniTask<IContract> GetContract<TView, TPresenter>(string key)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.instances.GetOrAdd(
                typeof(TView),
                () => this.addressableManager.LoadComponent<TView>(key).ContinueWith(view =>
                {
                    this.keys.Add(typeof(TView), key);
                    return new Contract(Instantiate(view), this.presenterFactory.Create(typeof(TPresenter)), this);
                })
            ).Cast<Contract, IContract>();
        }

        public UniTask<IContract> GetContract<TView, TPresenter>()
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.GetContract<TView, TPresenter>(typeof(TView).GetKeyAttribute());
        }
    }
}