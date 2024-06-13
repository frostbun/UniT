#nullable enable
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

        private readonly Dictionary<IActivity, IView[]>   activities       = new Dictionary<IActivity, IView[]>();
        private readonly List<IScreen>                    screensStack     = new List<IScreen>();
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

        TActivity IUIManager.RegisterActivity<TActivity>(TActivity activity) => this.RegisterActivity(activity);

        TActivity IUIManager.GetActivity<TActivity>(TActivity prefab) => this.GetActivity<TActivity>(prefab);

        TActivity IUIManager.GetActivity<TActivity>(string key)
        {
            var prefab = this.assetsManager.LoadComponent<IActivity>(key);
            return this.GetActivity<TActivity>(prefab, key);
        }

        #if UNIT_UNITASK
        async UniTask<TActivity> IUIManager.GetActivityAsync<TActivity>(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            var prefab = await this.assetsManager.LoadComponentAsync<IActivity>(key, progress, cancellationToken);
            return this.GetActivity<TActivity>(prefab, key);
        }
        #else
        IEnumerator IUIManager.GetActivityAsync<TActivity>(string key, Action<TActivity> callback, IProgress<float>? progress)
        {
            var prefab = default(IActivity)!;
            yield return this.assetsManager.LoadComponentAsync<IActivity>(
                key,
                result => prefab = result,
                progress
            );
            callback(this.GetActivity<TActivity>(prefab, key));
        }
        #endif

        #region Query

        IScreen? IUIManager.CurrentScreen => this.screensStack.LastOrDefault() is { CurrentStatus: IActivity.Status.Showing } screen ? screen : null;

        IEnumerable<IPopup> IUIManager.CurrentPopups => this.activities.Keys.OfType<IPopup>().Where(activity => activity.CurrentStatus is IActivity.Status.Showing);

        IEnumerable<IOverlay> IUIManager.CurrentOverlays => this.activities.Keys.OfType<IOverlay>().Where(activity => activity.CurrentStatus is IActivity.Status.Showing);

        #endregion

        #region UI Flow

        void IUIManager.Show(IActivityWithoutParams activity, bool force)
        {
            if (!this.TryHide(activity, force)) return;
            this.Show(activity);
        }

        void IUIManager.Show<TParams>(IActivityWithParams<TParams> activity, TParams @params, bool force)
        {
            if (!this.TryHide(activity, force)) return;
            activity.Params = @params;
            this.Show(activity);
        }

        void IUIManager.Hide(IActivity activity, bool autoStack)
        {
            if (activity is IScreen screen) this.screensStack.Remove(screen);
            this.Hide(activity, autoStack);
        }

        void IUIManager.Dispose(IActivity activity, bool autoStack)
        {
            if (activity is IScreen screen) this.screensStack.Remove(screen);
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

        private TActivity RegisterActivity<TActivity>(TActivity activity) where TActivity : IActivity
        {
            this.activities.TryAdd(activity, () =>
            {
                var views = activity.GetComponentsInChildren<IView>();
                views.ForEach(view => this.Initialize(view, activity));
                return views;
            });
            return activity;
        }

        private TActivity GetActivity<TActivity>(IActivity prefab, string? key = null) where TActivity : IActivity
        {
            return (TActivity)this.prefabToInstance.GetOrAdd(prefab, () =>
            {
                var activity = Object.Instantiate(prefab.GameObject, this.canvas.Hiddens, false).GetComponent<IActivity>();
                this.instanceToPrefab.Add(activity, prefab);
                if (key is { }) this.instanceToKey.Add(activity, key);
                this.RegisterActivity(activity);
                return activity;
            });
        }

        private bool TryHide(IActivity activity, bool force)
        {
            if (!force && activity.CurrentStatus is IActivity.Status.Showing) return false;
            this.Hide(activity, false);
            return true;
        }

        private void SetStackTop(IScreen screen)
        {
            var index = this.screensStack.IndexOf(screen);
            if (index is -1)
            {
                this.screensStack.Add(screen);
            }
            else
            {
                this.screensStack.RemoveRange(index + 1, this.screensStack.Count - index - 1);
            }
            this.activities.Keys
                .Where(other => other is not IOverlay)
                .Where(other => other.CurrentStatus is IActivity.Status.Showing)
                .SafeForEach(other => this.Hide(other, false));
        }

        private void Show(IActivity activity)
        {
            if (activity is IScreen screen) this.SetStackTop(screen);
            activity.Transform.SetParent(
                activity switch
                {
                    IScreen  => this.canvas.Screens,
                    IPopup   => this.canvas.Popups,
                    IOverlay => this.canvas.Overlays,
                    _        => throw new NotSupportedException($"Showing {activity.GetType().Name} is not supported"),
                },
                false
            );
            activity.Transform.SetAsLastSibling();
            this.logger.Debug($"{activity.Name} status: {activity.CurrentStatus = IActivity.Status.Showing}");
            this.activities[activity].ForEach(view => view.OnShow());
        }

        private void Hide(IActivity activity, bool autoStack)
        {
            if (activity.CurrentStatus is not IActivity.Status.Hidden)
            {
                this.logger.Debug($"{activity.Name} status: {activity.CurrentStatus = IActivity.Status.Hidden}");
                this.activities[activity].ForEach(view => view.OnHide());
                activity.Transform.SetParent(this.canvas.Hiddens, false);
            }
            if (autoStack && this.screensStack.LastOrDefault() is { CurrentStatus: IActivity.Status.Hidden } nextScreen)
            {
                this.Show(nextScreen);
            }
        }

        private void Dispose(IActivity activity, bool autoStack)
        {
            this.Hide(activity, autoStack);
            this.activities.Remove(activity);
            if (this.instanceToPrefab.TryRemove(activity, out var prefab))
            {
                this.prefabToInstance.Remove(prefab);
            }
            this.logger.Debug($"{activity.Name} status: {activity.CurrentStatus = IActivity.Status.Disposed}");
            Object.Destroy(activity.GameObject);
            if (this.instanceToKey.TryRemove(activity, out var key))
            {
                this.assetsManager.Unload(key);
            }
        }

        #endregion
    }
}