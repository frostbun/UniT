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
    using UniT.UI.View;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class UIManager : IUIManager
    {
        #region Constructor

        private readonly RootUICanvas   canvas;
        private readonly IInstantiator  instantiator;
        private readonly IAssetsManager assetsManager;
        private readonly ILogger        logger;

        private readonly HashSet<IActivity>               activities       = new HashSet<IActivity>();
        private readonly List<IActivity>                  activityStack    = new List<IActivity>();
        private readonly Dictionary<IActivity, IActivity> prefabToInstance = new Dictionary<IActivity, IActivity>();
        private readonly Dictionary<IActivity, IActivity> instanceToPrefab = new Dictionary<IActivity, IActivity>();
        private readonly Dictionary<IActivity, string>    instanceToKey    = new Dictionary<IActivity, string>();

        [Preserve]
        public UIManager(RootUICanvas canvas, IInstantiator instantiator, IAssetsManager assetsManager, ILoggerManager loggerManager)
        {
            this.canvas        = canvas;
            this.instantiator  = instantiator;
            this.assetsManager = assetsManager;
            this.logger        = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

        void IUIManager.Initialize(IView view, IActivity parent) => this.Initialize(view, parent);

        TActivity IUIManager.RegisterActivity<TActivity>(TActivity activity)
        {
            if (this.activities.Add(activity))
            {
                activity.GetComponentsInChildren<IView>().ForEach(view => this.Initialize(view, activity));
            }
            return activity;
        }

        TActivity IUIManager.GetActivity<TActivity>(TActivity prefab)
        {
            return this.GetActivity<TActivity>(prefab);
        }

        TActivity IUIManager.GetActivity<TActivity>(string key)
        {
            var prefab = this.assetsManager.LoadComponent<IActivity>(key);
            return this.GetActivity<TActivity>(prefab, key);
        }

        #if UNIT_UNITASK
        async UniTask<TActivity> IUIManager.GetActivityAsync<TActivity>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var prefab = await this.assetsManager.LoadComponentAsync<IActivity>(key, progress, cancellationToken);
            return this.GetActivity<TActivity>(prefab, key);
        }
        #else
        IEnumerator IUIManager.GetActivityAsync<TActivity>(string key, Action<TActivity> callback, IProgress<float> progress)
        {
            var prefab = default(IActivity);
            yield return this.assetsManager.LoadComponentAsync<IActivity>(
                key,
                result => prefab = result,
                progress
            );
            callback(this.GetActivity<TActivity>(prefab, key));
        }
        #endif

        #region Query

        IActivity IUIManager.StackingActivity => this.activityStack.LastOrDefault(activity => activity.CurrentStatus is IActivity.Status.Stacking);

        IActivity IUIManager.NextActivityInStack => this.activityStack.LastOrDefault(activity => activity.CurrentStatus is not IActivity.Status.Stacking);

        IEnumerable<IActivity> IUIManager.FloatingActivities => this.activities.Where(activity => activity.CurrentStatus is IActivity.Status.Floating);

        IEnumerable<IActivity> IUIManager.DockedActivities => this.activities.Where(activity => activity.CurrentStatus is IActivity.Status.Docked);

        #endregion

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

        private void Initialize(IView view, IActivity parent)
        {
            view.Manager  = this;
            view.Activity = parent;
            if (view is IHasPresenter owner)
            {
                var presenter = (IPresenter)this.instantiator.Instantiate(owner.PresenterType);
                presenter.Owner = owner;
                owner.Presenter = presenter;
            }
            view.OnInitialize();
            this.logger.Debug($"{view.Name} initialized");
        }

        private TActivity GetActivity<TActivity>(IActivity prefab, string key = null) where TActivity : IActivity
        {
            return (TActivity)this.prefabToInstance.GetOrAdd(prefab, () =>
            {
                var activity = Object.Instantiate(prefab.GameObject, this.canvas.HiddenActivities, false).GetComponent<IActivity>();
                activity.GetComponentsInChildren<IView>().ForEach(view => this.Initialize(view, activity));
                this.activities.Add(activity);
                this.instanceToPrefab.Add(activity, prefab);
                if (key is { }) this.instanceToKey.Add(activity, key);
                return activity;
            });
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
                this.activities
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
            if (removeFromStack) this.activityStack.Remove(activity);
            if (activity.CurrentStatus is IActivity.Status.Hidden) return;
            this.logger.Debug($"{activity.Name} status: {activity.CurrentStatus = IActivity.Status.Hidden}");
            activity.OnHide();
            activity.Transform.SetParent(this.canvas.HiddenActivities, false);
            if (autoStack && this.activityStack.LastOrDefault() is { CurrentStatus: not IActivity.Status.Stacking } nextActivity)
            {
                this.Show(nextActivity, IActivity.Status.Stacking);
            }
        }

        private void Dispose(IActivity activity, bool autoStack)
        {
            this.Hide(activity, true, autoStack);
            this.activities.Remove(activity);
            if (this.instanceToPrefab.TryRemove(activity, out var prefab))
            {
                this.prefabToInstance.Remove(prefab);
            }
            this.logger.Debug($"{activity.Name} status: {activity.CurrentStatus = IActivity.Status.Disposed}");
            activity.OnDispose();
            Object.Destroy(activity.GameObject);
            if (this.instanceToKey.TryRemove(activity, out var key))
            {
                this.assetsManager.Unload(key);
            }
        }

        #endregion
    }
}