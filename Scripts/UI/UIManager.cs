namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UniT.Instantiator;
    using UniT.Logging;
    using UniT.ResourcesManager;
    using UniT.UI.Activity;
    using UniT.UI.Presenter;
    using UniT.UI.UIElement;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public sealed class UIManager : MonoBehaviour, IUIManager, IHasLogger
    {
        #region Constructor

        private readonly RootUICanvas   canvas;
        private readonly IInstantiator  instantiator;
        private readonly IAssetsManager assetsManager;
        private readonly ILogger        logger;

        private readonly Dictionary<Type, IActivity> activities    = new Dictionary<Type, IActivity>();
        private readonly List<IActivity>             activityStack = new List<IActivity>();
        private readonly Dictionary<Type, string>    keys          = new Dictionary<Type, string>();

        [Preserve]
        public UIManager(RootUICanvas canvas, IInstantiator instantiator, IAssetsManager assetsManager, ILoggerFactory loggerFactory)
        {
            this.canvas        = canvas;
            this.instantiator  = instantiator;
            this.assetsManager = assetsManager;
            this.logger        = loggerFactory.Create(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        LogConfig IHasLogger.LogConfig => this.logger.Config;

        #region Public

        void IUIManager.Initialize<TUIElement>(TUIElement uiElement) => this.Initialize(uiElement);

        #region Query

        IActivity IUIManager.StackingActivity => this.activityStack.LastOrDefault(activity => activity.CurrentStatus is IActivity.Status.Stacking);

        IActivity IUIManager.NextActivityInStack => this.activityStack.LastOrDefault(activity => activity.CurrentStatus is not IActivity.Status.Stacking);

        IEnumerable<IActivity> IUIManager.FloatingActivities => this.activities.Values.Where(activity => activity.CurrentStatus is IActivity.Status.Floating);

        IEnumerable<IActivity> IUIManager.DockedActivities => this.activities.Values.Where(activity => activity.CurrentStatus is IActivity.Status.Docked);

        #endregion

        TActivity IUIManager.GetActivity<TActivity>(TActivity activity)
        {
            var initializedActivity = this.activities.GetOrAdd(activity.GetType(), () =>
            {
                activity.GetComponentsInChildren<IUIElement>().ForEach(this.Initialize);
                return activity;
            });
            if (!ReferenceEquals(initializedActivity, activity)) this.logger.Warning($"Found another instance of {activity.Name} in the manager. Using the cached instance.");
            return activity;
        }

        TActivity IUIManager.GetActivity<TActivity>(string key)
        {
            return (TActivity)this.activities.GetOrAdd(
                typeof(TActivity),
                () =>
                {
                    var prefab = this.assetsManager.Load<GameObject>(key);
                    this.keys.Add(typeof(TActivity), key);
                    return this.InstantiateActivity(prefab);
                }
            );
        }

        #if UNIT_UNITASK
        UniTask<TActivity> IUIManager.GetActivityAsync<TActivity>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return this.activities.GetOrAddAsync(
                typeof(TActivity),
                () => this.assetsManager.LoadAsync<GameObject>(key, progress, cancellationToken)
                    .ContinueWith(prefab =>
                    {
                        this.keys.Add(typeof(TActivity), key);
                        return this.InstantiateActivity(prefab);
                    })
            ).ContinueWith(activity => (TActivity)activity);
        }
        #endif

        #region UI Flow

        IActivity IUIManager.Stack(IActivityWithoutParams activity, bool force)
        {
            if (!force && activity.CurrentStatus is IActivity.Status.Stacking) return activity;
            this.Hide(activity, false, false);
            return this.Show(activity, IActivity.Status.Stacking);
        }

        IActivity IUIManager.Float(IActivityWithoutParams activity, bool force)
        {
            if (!force && activity.CurrentStatus is IActivity.Status.Floating) return activity;
            this.Hide(activity, false, false);
            return this.Show(activity, IActivity.Status.Floating);
        }

        IActivity IUIManager.Dock(IActivityWithoutParams activity, bool force)
        {
            if (!force && activity.CurrentStatus is IActivity.Status.Docked) return activity;
            this.Hide(activity, false, false);
            return this.Show(activity, IActivity.Status.Docked);
        }

        IActivity IUIManager.Stack<TParams>(IActivityWithParams<TParams> activity, TParams @params, bool force)
        {
            if (!force && activity.CurrentStatus is IActivity.Status.Stacking) return activity;
            this.Hide(activity, false, false);
            activity.Params = @params;
            return this.Show(activity, IActivity.Status.Stacking);
        }

        IActivity IUIManager.Float<TParams>(IActivityWithParams<TParams> activity, TParams @params, bool force)
        {
            if (!force && activity.CurrentStatus is IActivity.Status.Floating) return activity;
            this.Hide(activity, false, false);
            activity.Params = @params;
            return this.Show(activity, IActivity.Status.Floating);
        }

        IActivity IUIManager.Dock<TParams>(IActivityWithParams<TParams> activity, TParams @params, bool force)
        {
            if (!force && activity.CurrentStatus is IActivity.Status.Docked) return activity;
            this.Hide(activity, false, false);
            activity.Params = @params;
            return this.Show(activity, IActivity.Status.Docked);
        }

        void IUIManager.Hide(IActivity activity, bool removeFromStack, bool autoStack)
        {
            this.Hide(activity, removeFromStack, autoStack);
        }

        void IUIManager.Dispose(IActivity activity, bool autoStack)
        {
            this.Dispose(activity, autoStack);
        }

        #endregion

        #endregion

        #region Private

        private IActivity InstantiateActivity(GameObject prefab)
        {
            var activity = Instantiate(prefab, this.canvas.HiddenActivities, false).GetComponentOrThrow<IActivity>();
            activity.GetComponentsInChildren<IUIElement>().ForEach(this.Initialize);
            return activity;
        }

        private void Initialize(IUIElement uiElement)
        {
            uiElement.Manager = this;
            if (uiElement is IHasPresenter owner)
            {
                var presenter = (IPresenter)this.instantiator.Instantiate(owner.PresenterType);
                presenter.Owner = owner;
                owner.Presenter = presenter;
            }
            uiElement.OnInitialize();
            this.logger.Debug($"{uiElement.Name} initialized");
        }

        private IActivity Show(IActivity activity, IActivity.Status nextStatus)
        {
            if (nextStatus is IActivity.Status.Stacking)
            {
                var index = this.activityStack.IndexOf(activity);
                if (index is -1)
                {
                    this.activityStack.Add(activity);
                }
                else
                {
                    this.activityStack.RemoveRange(index + 1, this.activityStack.Count - index - 1);
                }
                this.activities.Values
                    .Where(other => other.CurrentStatus is IActivity.Status.Stacking or IActivity.Status.Floating)
                    .SafeForEach(other => this.Hide(other, false, false));
            }
            activity.Transform.SetParent(
                nextStatus switch
                {
                    IActivity.Status.Stacking => this.canvas.StackingActivities,
                    IActivity.Status.Floating => this.canvas.FloatingActivities,
                    IActivity.Status.Docked   => this.canvas.DockedActivities,
                    _                         => throw new ArgumentOutOfRangeException(nameof(nextStatus), nextStatus, null),
                },
                false
            );
            activity.Transform.SetAsLastSibling();
            this.logger.Debug($"{activity.Name} status: {activity.CurrentStatus = nextStatus}");
            activity.OnShow();
            return activity;
        }

        private void Hide(IActivity activity, bool removeFromStack, bool autoStack)
        {
            if (activity.CurrentStatus is IActivity.Status.Hidden) return;
            activity.Transform.SetParent(this.canvas.HiddenActivities, false);
            this.logger.Debug($"{activity.Name} status: {activity.CurrentStatus = IActivity.Status.Hidden}");
            activity.OnHide();
            if (removeFromStack)
            {
                this.activityStack.Remove(activity);
            }
            if (autoStack && this.activityStack.LastOrDefault() is { CurrentStatus: not IActivity.Status.Stacking } nextActivity)
            {
                this.Show(nextActivity, IActivity.Status.Stacking);
            }
        }

        private void Dispose(IActivity activity, bool autoStack)
        {
            this.Hide(activity, true, autoStack);
            this.activities.Remove(activity.GetType());
            this.logger.Debug($"{activity.Name} status: {activity.CurrentStatus = IActivity.Status.Disposed}");
            activity.OnDispose();
            Destroy(activity.GameObject);
            if (!this.keys.Remove(activity.GetType(), out var key)) return;
            this.assetsManager.Unload(key);
        }

        #endregion
    }
}