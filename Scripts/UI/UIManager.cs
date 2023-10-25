namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.UI.Activity;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public sealed class UIManager : MonoBehaviour, IUIManager
    {
        #region Constructor

        [SerializeField] private Transform _hiddenActivities;
        [SerializeField] private Transform _stackingActivities;
        [SerializeField] private Transform _floatingActivities;
        [SerializeField] private Transform _dockedActivities;

        private          IPresenter.Factory          _presenterFactory;
        private          IAssetManager               _assetManager;
        private          ILogger                     _logger;
        private readonly Dictionary<Type, IActivity> _activities    = new();
        private readonly List<IActivity>             _activityStack = new();
        private readonly Dictionary<Type, string>    _keys          = new();

        [Preserve]
        public UIManager Construct(IPresenter.Factory presenterFactory = null, IAssetManager assetManager = null, ILogger logger = null)
        {
            this._presenterFactory = presenterFactory ?? IPresenter.Factory.Default();
            this._assetManager     = assetManager ?? IAssetManager.Default();
            this._logger           = logger ?? ILogger.Default(this.GetType().Name);
            this._logger.Debug("Constructed");
            return this.DontDestroyOnLoad();
        }

        #endregion

        #region Finalizer

        public void Dispose() => Destroy(this);

        private void OnDestroy()
        {
            this._activities.Values.SafeForEach(activity => this.Dispose(activity, false));
            this._logger.Debug("Disposed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this._logger.Config;

        public TView Initialize<TView>(TView view) where TView : IView
        {
            Initialize(view);
            (view as Component)?.GetComponentsInChildren<IView>().ForEach(Initialize);
            this._logger.Debug($"Initialized {view.GetType().Name}");
            return view;

            void Initialize(IView view)
            {
                view.Manager = this;
                if (view is IViewWithPresenter viewWithPresenter)
                {
                    var presenter = this._presenterFactory.Create(viewWithPresenter.PresenterType);
                    presenter.View              = viewWithPresenter;
                    viewWithPresenter.Presenter = presenter;
                }
                view.OnInitialize();
            }
        }

        public IActivity StackingActivity => this._activityStack.LastOrDefault(activity => activity.CurrentStatus is IActivity.Status.Stacking);

        public IActivity NextActivityInStack => this._activityStack.LastOrDefault(activity => activity.CurrentStatus is not IActivity.Status.Stacking);

        public IEnumerable<IActivity> FloatingActivities => this._activities.Values.Where(activity => activity.CurrentStatus is IActivity.Status.Floating);

        public IEnumerable<IActivity> DockedActivities => this._activities.Values.Where(activity => activity.CurrentStatus is IActivity.Status.Docked);

        public IActivity GetActivity(IActivity activity)
        {
            var initializedActivity = this._activities.GetOrAdd(activity.GetType(), () => this.Initialize(activity));
            if (initializedActivity != activity) this._logger.Warning($"Found another instance of {activity.GetType().Name} in the manager. Using the cached instance.");
            return initializedActivity;
        }

        public UniTask<IActivity> GetActivity<TActivity>(string key = null) where TActivity : Component, IActivity
        {
            key ??= typeof(TActivity).GetKey();
            return this._activities.GetOrAddAsync(
                typeof(TActivity),
                () => this._assetManager.LoadComponent<TActivity>(key).ContinueWith(activityPrefab =>
                {
                    this._keys.Add(typeof(TActivity), key);
                    return (IActivity)this.Initialize(Instantiate(activityPrefab, this._hiddenActivities, false));
                })
            );
        }

        public IActivity Stack(IActivity activity, bool force = false) => this.Show(activity, force, IActivity.Status.Stacking);

        public IActivity Float(IActivity activity, bool force = false) => this.Show(activity, force, IActivity.Status.Floating);

        public IActivity Dock(IActivity activity, bool force = false) => this.Show(activity, force, IActivity.Status.Docked);

        public void Hide(IActivity activity, bool removeFromStack = true, bool autoStack = true)
        {
            if (activity.CurrentStatus is IActivity.Status.Hidden) return;
            activity.transform.SetParent(this._hiddenActivities, false);
            this._logger.Debug($"{activity.GetType().Name} status: {activity.CurrentStatus = IActivity.Status.Hidden}");
            activity.OnHide();
            if (removeFromStack)
            {
                this._activityStack.Remove(activity);
            }
            if (autoStack && this.StackingActivity is null && this.NextActivityInStack is { } nextActivity)
            {
                this.Stack(nextActivity);
            }
        }

        public void Dispose(IActivity activity, bool autoStack = true)
        {
            this.Hide(activity, true, autoStack);
            this._activities.Remove(activity.GetType());
            this._logger.Debug($"{activity.GetType().Name} status: {activity.CurrentStatus = IActivity.Status.Disposed}");
            activity.OnDispose();
            Destroy(activity.gameObject);
            if (!this._keys.Remove(activity.GetType(), out var key)) return;
            this._assetManager.Unload(key);
        }

        #endregion

        #region Private

        private IActivity Show(IActivity activity, bool force, IActivity.Status nextStatus)
        {
            if (!force && activity.CurrentStatus == nextStatus) return activity;
            this.Hide(activity, false, false);
            switch (nextStatus)
            {
                case IActivity.Status.Stacking:
                {
                    var index = this._activityStack.IndexOf(activity);
                    if (index == -1)
                    {
                        this._activityStack.Add(activity);
                    }
                    else
                    {
                        this._activityStack.RemoveRange(index + 1, this._activityStack.Count - index - 1);
                    }
                    this._activities.Values
                        .Where(other => other.CurrentStatus is IActivity.Status.Floating or IActivity.Status.Stacking)
                        .SafeForEach(other => this.Hide(other, false, false));
                    activity.transform.SetParent(this._stackingActivities, false);
                    break;
                }
                case IActivity.Status.Floating:
                {
                    activity.transform.SetParent(this._floatingActivities, false);
                    break;
                }
                case IActivity.Status.Docked:
                {
                    activity.transform.SetParent(this._dockedActivities, false);
                    break;
                }
            }
            activity.transform.SetAsLastSibling();
            this._logger.Debug($"{activity.GetType().Name} status: {activity.CurrentStatus = nextStatus}");
            activity.OnShow();
            return activity;
        }

        #endregion
    }
}