#nullable enable
namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using UniT.UI.Activity;
    using UniT.UI.View;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IUIManager
    {
        public void Initialize(IView view, IActivity parent);

        public TActivity RegisterActivity<TActivity>(TActivity activity) where TActivity : IActivity;

        #region Sync

        public TActivity GetActivity<TActivity>(TActivity prefab) where TActivity : IActivity;

        public TActivity GetActivity<TActivity>(string key) where TActivity : IActivity;

        public TActivity GetActivity<TActivity>() where TActivity : IActivity => this.GetActivity<TActivity>(typeof(TActivity).GetKey());

        #endregion

        #region Async

        #if UNIT_UNITASK
        public UniTask<TActivity> GetActivityAsync<TActivity>(string key, IProgress<float>? progress = null, CancellationToken cancellationToken = default) where TActivity : IActivity;

        public UniTask<TActivity> GetActivityAsync<TActivity>(IProgress<float>? progress = null, CancellationToken cancellationToken = default) where TActivity : IActivity => this.GetActivityAsync<TActivity>(typeof(TActivity).GetKey(), progress, cancellationToken);
        #else
        public IEnumerator GetActivityAsync<TActivity>(string key, Action<TActivity> callback, IProgress<float>? progress = null) where TActivity : IActivity;

        public IEnumerator GetActivityAsync<TActivity>(Action<TActivity> callback, IProgress<float>? progress = null) where TActivity : IActivity => this.GetActivityAsync(typeof(TActivity).GetKey(), callback, progress);
        #endif

        #endregion

        #region Query

        public IScreen? CurrentScreen { get; }

        public IEnumerable<IPopup> CurrentPopups { get; }

        public IEnumerable<IOverlay> CurrentOverlays { get; }

        #endregion

        #region UI Flow

        public void Show(IActivityWithoutParams activity, bool force = false);

        public void Show<TParams>(IActivityWithParams<TParams> activity, TParams @params, bool force = true);

        public void Hide(IActivity activity, bool autoStack = true);

        public void Dispose(IActivity activity, bool autoStack = true);

        #endregion
    }
}