namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public interface IViewManager
    {
        public interface IViewInstance : IDisposable
        {
            public IViewManager Manager { get; }

            public ViewStatus CurrentStatus { get; }

            public IViewInstance BindModel(object model);

            public void Stack();

            public void Float();

            public void Detach();

            public void Hide();
        }

        public ILogger Logger { get; }

        public IViewInstance StackingView { get; }

        public IEnumerable<IViewInstance> FloatingViews { get; }

        public IEnumerable<IViewInstance> DetachedViews { get; }

        public IViewInstance GetView<TView, TPresenter>(TView view)
            where TView : Component, IView
            where TPresenter : IPresenter;

        public UniTask<IViewInstance> GetView<TView, TPresenter>(string key)
            where TView : Component, IView
            where TPresenter : IPresenter;

        public UniTask<IViewInstance> GetView<TView, TPresenter>()
            where TView : Component, IView
            where TPresenter : IPresenter;
    }

    public enum ViewStatus
    {
        Disposed,
        Hidden,
        Stacking,
        Floating,
        Detached,
    }
}