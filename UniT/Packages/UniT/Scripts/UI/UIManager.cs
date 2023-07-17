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
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class UIManager : MonoBehaviour, IUIManager
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

            public readonly  IView                      view;
            private readonly UIManager                  manager;
            private readonly Dictionary<string, object> extras;

            public Contract(IView view, IPresenter presenter, UIManager manager)
            {
                view.Contract  = presenter.Contract = this;
                view.Presenter = presenter;
                this.view      = presenter.View = view;
                this.manager   = manager;
                this.extras    = new();

                this.view.transform.SetParent(this.manager.hiddenViews, false);
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

            public void Stack(bool force = false)
            {
                if (!force && this.CurrentStatus is IContract.Status.Stacking) return;
                this.AddToStack();
                this.Show_Internal(IContract.Status.Stacking);
            }

            public void Float(bool force = false)
            {
                if (!force && this.CurrentStatus is IContract.Status.Floating) return;
                this.Show_Internal(IContract.Status.Floating);
            }

            public void Dock(bool force = false)
            {
                if (!force && this.CurrentStatus is IContract.Status.Docked) return;
                this.Show_Internal(IContract.Status.Docked);
            }

            public void Hide(bool autoStack = true)
            {
                this.Hide_Internal();
                this.RemoveFromStack(autoStack);
            }

            public void Dispose(bool autoStack = true)
            {
                this.Hide(autoStack);
                this.manager.contracts.Remove(this.view.GetType());

                this.CurrentStatus = IContract.Status.Disposed;
                this.view.Dispose();
                Destroy(this.view.gameObject);

                if (this.manager.keys.Remove(this.view.GetType(), out var key))
                {
                    this.manager.assetsManager.Unload(key);
                }
            }

            private void Show_Internal(IContract.Status status)
            {
                this.Hide_Internal();
                this.view.transform.SetParent(
                    status switch
                    {
                        IContract.Status.Stacking => this.manager.stackingViews,
                        IContract.Status.Floating => this.manager.floatingViews,
                        IContract.Status.Docked   => this.manager.dockedViews,
                        _                         => throw new ArgumentOutOfRangeException(nameof(status), status, null),
                    },
                    false
                );
                this.view.transform.SetAsLastSibling();
                this.CurrentStatus = status;
                this.view.Show();
            }

            private void Hide_Internal()
            {
                if (this.CurrentStatus is IContract.Status.Disposed) throw this.manager.Logger.Exception(new ObjectDisposedException(this.view.GetType().Name));
                if (this._currentStatus is IContract.Status.Hidden) return;
                this.view.transform.SetParent(this.manager.hiddenViews, false);
                this.CurrentStatus = IContract.Status.Hidden;
                this.view.Hide();
                this.extras.Clear();
            }

            private void AddToStack()
            {
                var index = this.manager.stack.IndexOf(this);
                if (index == -1)
                {
                    this.manager.stack.Add(this);
                }
                else
                {
                    this.manager.stack.RemoveRange(index + 1, this.manager.stack.Count - index - 1);
                }
                this.manager.contracts.Values.ToArray()
                    .Where(contract => contract.CurrentStatus is IContract.Status.Floating or IContract.Status.Stacking)
                    .ForEach(contract => contract.Hide_Internal());
            }

            private void RemoveFromStack(bool autoStack)
            {
                this.manager.stack.Remove(this);
                if (!autoStack) return;
                if (this.manager.StackingContract is not null) return;
                if (this.manager.stack.Count < 1) return;
                this.manager.stack[^1].Stack();
            }
        }

        [SerializeField]
        private RectTransform hiddenViews;

        [SerializeField]
        private RectTransform stackingViews;

        [SerializeField]
        private RectTransform floatingViews;

        [SerializeField]
        private RectTransform dockedViews;

        public ILogger Logger { get; private set; }

        private          IAssetsManager             assetsManager;
        private          IPresenter.IFactory        presenterFactory;
        private readonly Dictionary<Type, Contract> contracts = new();
        private readonly List<Contract>             stack     = new();
        private readonly Dictionary<Type, string>   keys      = new();

        public UIManager Construct(IAssetsManager assetsManager = null, IPresenter.IFactory presenterFactory = null, ILogger logger = null)
        {
            this.assetsManager    = assetsManager ?? IAssetsManager.Factory.Default();
            this.presenterFactory = presenterFactory ?? IPresenter.IFactory.Factory.Default();
            this.Logger           = logger ?? ILogger.Factory.Default(this.GetType().Name);
            this.DontDestroyOnLoad();
            return this;
        }

        public IContract StackingContract => this.stack.LastOrDefault(contract => contract.CurrentStatus is IContract.Status.Stacking);

        public IContract NextStackingContract => this.stack.LastOrDefault(contract => contract.CurrentStatus is not IContract.Status.Stacking);

        public IEnumerable<IContract> FloatingContracts => this.contracts.Values.Where(contract => contract.CurrentStatus is IContract.Status.Floating).OrderByDescending(contract => contract.view.transform.GetSiblingIndex());

        public IEnumerable<IContract> DockedContracts => this.contracts.Values.Where(contract => contract.CurrentStatus is IContract.Status.Docked).OrderByDescending(contract => contract.view.transform.GetSiblingIndex());

        public IContract GetContract<TView, TPresenter>(TView view)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.contracts.GetOrAdd(
                typeof(TView),
                () => new(view, this.presenterFactory.Create(typeof(TPresenter)), this)
            );
        }

        public UniTask<IContract> GetContract<TView, TPresenter>(string key)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.contracts.GetOrAdd(
                typeof(TView),
                () => this.assetsManager.LoadComponent<TView>(key).ContinueWith(view =>
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
            return this.GetContract<TView, TPresenter>(typeof(TView).GetKey());
        }
    }
}