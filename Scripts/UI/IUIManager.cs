namespace UniT.UI
{
    using System.Collections.Generic;
    using UniT.UI.Activity;
    using UniT.UI.View;
    using UniT.Utilities;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IUIManager
    {
        public void Initialize(IView view, IActivity parent);

        public TActivity GetActivity<TActivity>(TActivity activity) where TActivity : IActivity;

        public TActivity GetActivity<TActivity>(string key) where TActivity : IActivity;

        #region Query

        public IActivity StackingActivity { get; }

        public IActivity NextActivityInStack { get; }

        public IEnumerable<IActivity> FloatingActivities { get; }

        public IEnumerable<IActivity> DockedActivities { get; }

        #endregion

        #region UI Flow

        public IActivity Stack(IActivityWithoutParams activity, bool force = false);

        public IActivity Float(IActivityWithoutParams activity, bool force = false);

        public IActivity Dock(IActivityWithoutParams activity, bool force = false);

        public IActivity Stack<TParams>(IActivityWithParams<TParams> activity, TParams @params, bool force = true);

        public IActivity Float<TParams>(IActivityWithParams<TParams> activity, TParams @params, bool force = true);

        public IActivity Dock<TParams>(IActivityWithParams<TParams> activity, TParams @params, bool force = true);

        public void Hide(IActivity activity, bool removeFromStack = true, bool autoStack = true);

        public void Dispose(IActivity activity, bool autoStack = true);

        #endregion

        #region Implicit Key

        #if UNITY_2021_2_OR_NEWER
        public TActivity GetActivity<TActivity>() where TActivity : IActivity => this.GetActivity<TActivity>(typeof(TActivity).GetKey());
        #endif

        #endregion

        #region Async

        #if UNIT_UNITASK
        public UniTask<TActivity> GetActivityAsync<TActivity>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default) where TActivity : IActivity;

        #if UNITY_2021_2_OR_NEWER
        public UniTask<TActivity> GetActivityAsync<TActivity>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where TActivity : IActivity => this.GetActivityAsync<TActivity>(typeof(TActivity).GetKey(), progress, cancellationToken);
        #endif
        #endif

        #endregion
    }
}