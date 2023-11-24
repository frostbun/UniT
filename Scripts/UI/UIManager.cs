namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.ResourcesManager;
    using UniT.UI.Activity;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public sealed class UIManager : MonoBehaviour, IUIManager
    {
        #region Constructor

        [SerializeField] private Transform hiddenActivities;
        [SerializeField] private Transform stackingActivities;
        [SerializeField] private Transform floatingActivities;
        [SerializeField] private Transform dockedActivities;

        private          IPresenter.Factory          presenterFactory;
        private          IAssetsManager              assetsManager;
        private          ILogger                     logger;
        private readonly Dictionary<Type, IActivity> activities    = new();
        private readonly List<IActivity>             activityStack = new();
        private readonly Dictionary<Type, string>    keys          = new();

        [Preserve]
        public UIManager Construct(IPresenter.Factory presenterFactory = null, IAssetsManager assetsManager = null, ILogger logger = null)
        {
            this.presenterFactory = presenterFactory ?? IPresenter.Factory.Default();
            this.assetsManager    = assetsManager ?? IAssetsManager.Default();
            this.logger           = logger ?? ILogger.Default(nameof(UIManager));
            this.logger.Debug("Constructed");
            return this.DontDestroyOnLoad();
        }

        #endregion

        #region Finalizer

        public void Dispose() => Destroy(this);

        private void OnDestroy()
        {
            this.activities.Values.SafeForEach(activity => this.Dispose(activity, false));
            this.logger.Debug("Disposed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this.logger.Config;

        public TView Initialize<TView>(TView view) where TView : IView
        {
            Initialize(view);
            (view as Component)?.GetComponentsInChildren<IView>().ForEach(Initialize);
            this.logger.Debug($"Initialized {view.GetType().Name}");
            return view;

            void Initialize(IView view)
            {
                view.Manager = this;
                if (view is IViewWithPresenter viewWithPresenter)
                {
                    var presenter = this.presenterFactory.Create(viewWithPresenter.PresenterType);
                    presenter.View              = viewWithPresenter;
                    viewWithPresenter.Presenter = presenter;
                }
                view.OnInitialize();
            }
        }

        public IActivity StackingActivity => this.activityStack.LastOrDefault(activity => activity.CurrentStatus is IActivity.Status.Stacking);

        public IActivity NextActivityInStack => this.activityStack.LastOrDefault(activity => activity.CurrentStatus is not IActivity.Status.Stacking);

        public IEnumerable<IActivity> FloatingActivities => this.activities.Values.Where(activity => activity.CurrentStatus is IActivity.Status.Floating);

        public IEnumerable<IActivity> DockedActivities => this.activities.Values.Where(activity => activity.CurrentStatus is IActivity.Status.Docked);

        public IActivity GetActivity(IActivity activity)
        {
            var initializedActivity = this.activities.GetOrAdd(activity.GetType(), () => this.Initialize(activity));
            if (initializedActivity != activity) this.logger.Warning($"Found another instance of {activity.GetType().Name} in the manager. Using the cached instance.");
            return initializedActivity;
        }

        public UniTask<IActivity> GetActivity<TActivity>(string key = null) where TActivity : Component, IActivity
        {
            key ??= typeof(TActivity).GetKey();
            return this.activities.GetOrAddAsync(
                typeof(TActivity),
                () => this.assetsManager.LoadComponentAsync<TActivity>(key).ContinueWith(activityPrefab =>
                {
                    this.keys.Add(typeof(TActivity), key);
                    return (IActivity)this.Initialize(Instantiate(activityPrefab, this.hiddenActivities, false));
                })
            );
        }

        public IActivity Stack(IActivity activity, bool force = false) => this.Show(activity, force, IActivity.Status.Stacking);

        public IActivity Float(IActivity activity, bool force = false) => this.Show(activity, force, IActivity.Status.Floating);

        public IActivity Dock(IActivity activity, bool force = false) => this.Show(activity, force, IActivity.Status.Docked);

        public void Hide(IActivity activity, bool removeFromStack = true, bool autoStack = true)
        {
            if (activity.CurrentStatus is IActivity.Status.Hidden) return;
            activity.transform.SetParent(this.hiddenActivities, false);
            this.logger.Debug($"{activity.GetType().Name} status: {activity.CurrentStatus = IActivity.Status.Hidden}");
            activity.OnHide();
            if (removeFromStack)
            {
                this.activityStack.Remove(activity);
            }
            if (autoStack && this.StackingActivity is null && this.NextActivityInStack is { } nextActivity)
            {
                this.Stack(nextActivity);
            }
        }

        public void Dispose(IActivity activity, bool autoStack = true)
        {
            this.Hide(activity, true, autoStack);
            this.activities.Remove(activity.GetType());
            this.logger.Debug($"{activity.GetType().Name} status: {activity.CurrentStatus = IActivity.Status.Disposed}");
            activity.OnDispose();
            Destroy(activity.gameObject);
            if (!this.keys.Remove(activity.GetType(), out var key)) return;
            this.assetsManager.Unload(key);
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
                    var index = this.activityStack.IndexOf(activity);
                    if (index == -1)
                    {
                        this.activityStack.Add(activity);
                    }
                    else
                    {
                        this.activityStack.RemoveRange(index + 1, this.activityStack.Count - index - 1);
                    }
                    this.activities.Values
                        .Where(other => other.CurrentStatus is IActivity.Status.Floating or IActivity.Status.Stacking)
                        .SafeForEach(other => this.Hide(other, false, false));
                    activity.transform.SetParent(this.stackingActivities, false);
                    break;
                }
                case IActivity.Status.Floating:
                {
                    activity.transform.SetParent(this.floatingActivities, false);
                    break;
                }
                case IActivity.Status.Docked:
                {
                    activity.transform.SetParent(this.dockedActivities, false);
                    break;
                }
            }
            activity.transform.SetAsLastSibling();
            this.logger.Debug($"{activity.GetType().Name} status: {activity.CurrentStatus = nextStatus}");
            activity.OnShow();
            return activity;
        }

        #endregion
    }
}