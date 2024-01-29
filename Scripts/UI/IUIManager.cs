namespace UniT.UI
{
    using System.Collections.Generic;
    using UniT.Logging;
    using UniT.UI.Activity;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IUIManager : IHasLogger
    {
        public TView Initialize<TView>(TView view) where TView : IView;

        public IActivity StackingActivity { get; }

        public IActivity NextActivityInStack { get; }

        public IEnumerable<IActivity> FloatingActivities { get; }

        public IEnumerable<IActivity> DockedActivities { get; }

        public IActivity GetActivity(IActivity activity);

        public IActivity GetActivity(string key);

        public IActivity GetActivity<TActivity>() where TActivity : IActivity => this.GetActivity(typeof(TActivity).GetKey());

        public IActivity Stack(IActivity activity, bool force = false);

        public IActivity Float(IActivity activity, bool force = false);

        public IActivity Dock(IActivity activity, bool force = false);

        public void Hide(IActivity activity, bool removeFromStack = true, bool autoStack = true);

        public void Dispose(IActivity activity, bool autoStack = true);

        #region Async

        #if UNIT_UNITASK
        public UniTask<IActivity> GetActivityAsync(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask<IActivity> GetActivityAsync<TActivity>() where TActivity : IActivity => this.GetActivityAsync(typeof(TActivity).GetKey());
        #endif

        #endregion
    }
}