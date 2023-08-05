namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Extensions;
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

        private          IPresenter.Factory        _presenterFactory;
        private          IAssetsManager            _assetsManager;
        private          ILogger                   _logger;
        private readonly Dictionary<Type, IScreen> _screens     = new();
        private readonly List<IScreen>             _screenStack = new();
        private readonly Dictionary<Type, string>  _keys        = new();

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
            if (view is IViewWithPresenter viewWithPresenter)
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

        public IScreen StackingScreen => this._screenStack.LastOrDefault(screen => screen.CurrentStatus is IScreen.Status.Stacking);

        public IScreen NextScreenInStack => this._screenStack.LastOrDefault(screen => screen.CurrentStatus is not IScreen.Status.Stacking);

        public IEnumerable<IScreen> FloatingScreens => this._screens.Values.Where(screen => screen.CurrentStatus is IScreen.Status.Floating);

        public IEnumerable<IScreen> DockedScreens => this._screens.Values.Where(screen => screen.CurrentStatus is IScreen.Status.Docked);

        public IScreen GetScreen(IScreen screen)
        {
            var initializedScreen = this._screens.GetOrAdd(screen.GetType(), () => this.Initialize(screen));
            if (initializedScreen != screen) this._logger.Warning($"Found another instance of {screen.GetType().Name} in the manager. Using the cached instance.");
            return initializedScreen;
        }

        public UniTask<IScreen> GetScreen<TScreen>(string key) where TScreen : Component, IScreen
        {
            return this._screens.GetOrAdd(
                typeof(TScreen),
                () => this._assetsManager.LoadComponent<TScreen>(key).ContinueWith(screenPrefab =>
                {
                    this._keys.Add(typeof(TScreen), key);
                    return (IScreen)this.Initialize(Instantiate(screenPrefab, this._hiddenScreens, false));
                })
            );
        }

        public UniTask<IScreen> GetScreen<TScreen>() where TScreen : Component, IScreen
        {
            return this.GetScreen<TScreen>(typeof(TScreen).GetKey());
        }

        public void Stack(IScreen screen, bool force = false) => this.Show(screen, force, IScreen.Status.Stacking);

        public void Float(IScreen screen, bool force = false) => this.Show(screen, force, IScreen.Status.Floating);

        public void Dock(IScreen screen, bool force = false) => this.Show(screen, force, IScreen.Status.Docked);

        public void Hide(IScreen screen, bool removeFromStack = true, bool autoStack = true)
        {
            if (screen.CurrentStatus is IScreen.Status.Hidden) return;
            screen.transform.SetParent(this._hiddenScreens, false);
            this._logger.Debug($"{screen.GetType().Name} status: {screen.CurrentStatus = IScreen.Status.Hidden}");
            screen.OnHide();
            if (removeFromStack) this.RemoveFromStack(screen);
            if (autoStack) this.StackNextScreen();
        }

        public void Dispose(IScreen screen, bool autoStack = true)
        {
            this.Hide(screen, true, autoStack);
            this._screens.Remove(screen.GetType());
            this._logger.Debug($"{screen.GetType().Name} status: {screen.CurrentStatus = IScreen.Status.Disposed}");
            screen.OnDispose();
            Destroy(screen.gameObject);
            if (!this._keys.Remove(screen.GetType(), out var key)) return;
            this._assetsManager.Unload(key);
        }

        private void Show(IScreen screen, bool force, IScreen.Status nextStatus)
        {
            if (!force && screen.CurrentStatus == nextStatus) return;
            this.Hide(screen, false, false);
            switch (nextStatus)
            {
                case IScreen.Status.Stacking:
                {
                    this.AddToStack(screen);
                    this.HideUndockedScreens();
                    screen.transform.SetParent(this._stackingScreens, false);
                    break;
                }
                case IScreen.Status.Floating:
                {
                    screen.transform.SetParent(this._floatingScreens, false);
                    break;
                }
                case IScreen.Status.Docked:
                {
                    screen.transform.SetParent(this._dockedScreens, false);
                    break;
                }
            }
            screen.transform.SetAsLastSibling();
            this._logger.Debug($"{screen.GetType().Name} status: {screen.CurrentStatus = nextStatus}");
            screen.OnShow();
        }

        private void AddToStack(IScreen screen)
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

        private void RemoveFromStack(IScreen screen)
        {
            this._screenStack.Remove(screen);
        }

        private void StackNextScreen()
        {
            if (this.StackingScreen is not null) return;
            var nextScreen = this.NextScreenInStack;
            if (nextScreen is null) return;
            this.Stack(nextScreen);
        }

        private void HideUndockedScreens()
        {
            this._screens.Values.ToArray()
                .Where(screen => screen.CurrentStatus is IScreen.Status.Floating or IScreen.Status.Stacking)
                .ForEach(screen => this.Hide(screen, false, false));
        }
    }
}