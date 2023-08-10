namespace UniT.UI
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UniT.UI.Activity;
    using UnityEngine;

    public interface IUIManager
    {
        public LogConfig LogConfig { get; }

        public TView Initialize<TView>(TView view) where TView : IView;

        public IActivity StackingActivity { get; }

        public IActivity NextActivityInStack { get; }

        public IEnumerable<IActivity> FloatingActivities { get; }

        public IEnumerable<IActivity> DockedActivities { get; }

        public IActivity GetActivity(IActivity activity);

        public UniTask<IActivity> GetActivity<TActivity>(string key) where TActivity : Component, IActivity;

        public UniTask<IActivity> GetActivity<TActivity>() where TActivity : Component, IActivity;

        public void Stack(IActivity activity, bool force = false);

        public void Float(IActivity activity, bool force = false);

        public void Dock(IActivity activity, bool force = false);

        public void Hide(IActivity activity, bool removeFromStack = true, bool autoStack = true);

        public void Dispose(IActivity activity, bool autoStack = true);
    }
}