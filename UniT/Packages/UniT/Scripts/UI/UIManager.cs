namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Extensions;
    using UniT.Extensions.UniTask;
    using UniT.Logging;
    using UniT.UI.Screen;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public class UIManager : MonoBehaviour, IUIManager
    {
        [SerializeField] private RectTransform _hiddenScreens;
        [SerializeField] private RectTransform _stackingScreens;
        [SerializeField] private RectTransform _floatingScreens;
        [SerializeField] private RectTransform _dockedScreens;

        private          IPresenter.Factory            _presenterFactory;
        private          IAssetsManager                _assetsManager;
        private          ILogger                       _logger;
        private readonly Dictionary<Type, IScreenView> _screens     = new();
        private readonly List<IScreenView>             _screenStack = new();
        private readonly Dictionary<Type, string>      _keys        = new();

        [Preserve]
        public UIManager Construct(IPresenter.Factory presenterFactory = null, IAssetsManager assetsManager = null, ILogger logger = null)
        {
            this._presenterFactory = presenterFactory ?? IPresenter.Factory.Default();
            this._assetsManager    = assetsManager ?? IAssetsManager.Default();
            this._logger           = logger ?? ILogger.Default(this.GetType().Name);
            return this.DontDestroyOnLoad();
        }

        public LogConfig LogConfig => this._logger.Config;

        public TView Initialize<TView>(TView view) where TView : IView
        {
            if (view is IScreenView screen)
            {
                if (!this._screens.TryAdd(view.GetType(), screen))
                {
                    this._logger.Warning($"ScreenView {view.GetType().Name} already initialized");
                    return view;
                }
                screen.transform.SetParent(this._hiddenScreens, false);
                screen.CurrentStatus = IScreenView.Status.Hidden;
            }
            if (view is IScreenViewWithPresenter viewWithPresenter)
            {
                var presenter = this._presenterFactory.Create(viewWithPresenter.PresenterType);
                presenter.View              = view;
                viewWithPresenter.Presenter = presenter;
            }
            view.Manager = this;
            this._logger.Debug($"Initialized {view.GetType().Name}");
            view.OnInitialize();
            return view;
        }

        public IScreenView StackingScreen => this._screenStack.LastOrDefault(view => view.CurrentStatus is IScreenView.Status.Stacking);

        public IScreenView NextScreenInStack => this._screenStack.LastOrDefault(view => view.CurrentStatus is not IScreenView.Status.Stacking);

        public IEnumerable<IScreenView> FloatingScreens => this._screens.Values.Where(view => view.CurrentStatus is IScreenView.Status.Floating);

        public IEnumerable<IScreenView> DockedScreens => this._screens.Values.Where(view => view.CurrentStatus is IScreenView.Status.Docked);

        public UniTask<IScreenView> GetScreen<TScreenView>(string key) where TScreenView : Component, IScreenView
        {
            return this._screens.GetOrAdd(
                typeof(TScreenView),
                () => this._assetsManager.LoadComponent<TScreenView>(key).ContinueWith(screenPrefab =>
                {
                    this._keys.Add(typeof(TScreenView), key);
                    return (IScreenView)this.Initialize(Instantiate(screenPrefab));
                })
            );
        }

        public UniTask<IScreenView> GetScreen<TScreenView>() where TScreenView : Component, IScreenView
        {
            return this.GetScreen<TScreenView>(typeof(TScreenView).GetKey());
        }

        public void Stack(IScreenView screen, bool force = false) => this.Show(screen, force, IScreenView.Status.Stacking);

        public void Float(IScreenView screen, bool force = false) => this.Show(screen, force, IScreenView.Status.Floating);

        public void Dock(IScreenView screen, bool force = false) => this.Show(screen, force, IScreenView.Status.Docked);

        public void Hide(IScreenView screen, bool removeFromStack = true, bool autoStack = true)
        {
            if (screen.CurrentStatus is IScreenView.Status.Hidden) return;
            screen.transform.SetParent(this._hiddenScreens, false);
            this._logger.Debug($"{screen.GetType().Name} status: {screen.CurrentStatus = IScreenView.Status.Hidden}");
            screen.OnHide();
            if (removeFromStack) this.RemoveFromStack(screen);
            if (autoStack) this.StackNextScreen();
        }

        public void Dispose(IScreenView screen, bool autoStack = true)
        {
            this.Hide(screen, true, autoStack);
            Destroy(screen.gameObject);
            this._screens.Remove(screen.GetType());
            if (!this._keys.Remove(screen.GetType(), out var key)) return;
            this._assetsManager.Unload(key);
            this._logger.Debug($"{screen.GetType().Name} status: {screen.CurrentStatus = IScreenView.Status.Disposed}");
            screen.OnDispose();
        }

        private void Show(IScreenView screen, bool force, IScreenView.Status nextStatus)
        {
            if (!force && screen.CurrentStatus == nextStatus) return;
            this.Hide(screen, false, false);
            switch (nextStatus)
            {
                case IScreenView.Status.Stacking:
                {
                    this.AddToStack(screen);
                    this.HideUndockedScreens();
                    screen.transform.SetParent(this._stackingScreens, false);
                    break;
                }
                case IScreenView.Status.Floating:
                {
                    screen.transform.SetParent(this._floatingScreens, false);
                    break;
                }
                case IScreenView.Status.Docked:
                {
                    screen.transform.SetParent(this._dockedScreens, false);
                    break;
                }
            }
            screen.transform.SetAsLastSibling();
            this._logger.Debug($"{screen.GetType().Name} status: {screen.CurrentStatus = nextStatus}");
            screen.OnShow();
        }

        private void AddToStack(IScreenView screen)
        {
            var index = this._screenStack.IndexOf(screen);
            if (index == -1)
            {
                this._screenStack.Add(screen);
            }
            else
            {
                this._screenStack.RemoveRange(index + 1, this._screenStack.Count - index - 1);
            }
        }

        private void RemoveFromStack(IScreenView screen)
        {
            this._screenStack.Remove(screen);
        }

        private void StackNextScreen()
        {
            if (this.StackingScreen is not null) return;
            this.NextScreenInStack?.Stack();
        }

        private void HideUndockedScreens()
        {
            this._screens.Values.ToArray()
                .Where(screen => screen.CurrentStatus is IScreenView.Status.Floating or IScreenView.Status.Stacking)
                .ForEach(screen => this.Hide(screen, false, false));
        }
    }
}