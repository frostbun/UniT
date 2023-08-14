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

    public class UIManager : MonoBehaviour, IUIManager
    {
        #region Constructor

        [SerializeField] private RectTransform _hiddenActivities;
        [SerializeField] private RectTransform _stackingActivities;
        [SerializeField] private RectTransform _floatingActivities;
        [SerializeField] private RectTransform _dockedActivities;

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
            return this.DontDestroyOnLoad();
        }

        #endregion

        #region Public

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
            return this._activities.GetOrAdd(
                typeof(TActivity),
                () => this._assetManager.LoadComponent<TActivity>(key).ContinueWith(activityPrefab =>
                {
                    this._keys.Add(typeof(TActivity), key);
                    return (IActivity)this.Initialize(Instantiate(activityPrefab, this._hiddenActivities, false));
                })
            );
        }

        public void Stack(IActivity activity, bool force = false) => this.Show(activity, force, IActivity.Status.Stacking);

        public void Float(IActivity activity, bool force = false) => this.Show(activity, force, IActivity.Status.Floating);

        public void Dock(IActivity activity, bool force = false) => this.Show(activity, force, IActivity.Status.Docked);

        public void Hide(IActivity activity, bool removeFromStack = true, bool autoStack = true)
        {
            if (activity.CurrentStatus is IActivity.Status.Hidden) return;
            activity.Transform.SetParent(this._hiddenActivities, false);
            this._logger.Debug($"{activity.GetType().Name} status: {activity.CurrentStatus = IActivity.Status.Hidden}");
            activity.OnHide();
            if (removeFromStack) this.RemoveFromStack(activity);
            if (autoStack) this.StackNextActivity();
        }

        public void Dispose(IActivity activity, bool autoStack = true)
        {
            this.Hide(activity, true, autoStack);
            this._activities.Remove(activity.GetType());
            this._logger.Debug($"{activity.GetType().Name} status: {activity.CurrentStatus = IActivity.Status.Disposed}");
            activity.OnDispose();
            Destroy(activity.GameObject);
            if (!this._keys.Remove(activity.GetType(), out var key)) return;
            this._assetManager.Unload(key);
        }

        #endregion

        #region Private

        private void Show(IActivity activity, bool force, IActivity.Status nextStatus)
        {
            if (!force && activity.CurrentStatus == nextStatus) return;
            this.Hide(activity, false, false);
            switch (nextStatus)
            {
                case IActivity.Status.Stacking:
                {
                    this.AddToStack(activity);
                    this.HideUndockedActivities();
                    activity.Transform.SetParent(this._stackingActivities, false);
                    break;
                }
                case IActivity.Status.Floating:
                {
                    activity.Transform.SetParent(this._floatingActivities, false);
                    break;
                }
                case IActivity.Status.Docked:
                {
                    activity.Transform.SetParent(this._dockedActivities, false);
                    break;
                }
            }
            activity.Transform.SetAsLastSibling();
            this._logger.Debug($"{activity.GetType().Name} status: {activity.CurrentStatus = nextStatus}");
            activity.OnShow();
        }

        private void AddToStack(IActivity activity)
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
        }

        private void RemoveFromStack(IActivity activity)
        {
            this._activityStack.Remove(activity);
        }

        private void StackNextActivity()
        {
            if (this.StackingActivity is not null) return;
            var nextActivity = this.NextActivityInStack;
            if (nextActivity is null) return;
            this.Stack(nextActivity);
        }

        private void HideUndockedActivities()
        {
            this._activities.Values
                .Where(activity => activity.CurrentStatus is IActivity.Status.Floating or IActivity.Status.Stacking)
                .SafeForEach(activity => this.Hide(activity, false, false));
        }

        #endregion
    }
}