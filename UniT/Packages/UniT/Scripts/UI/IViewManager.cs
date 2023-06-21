namespace UniT.UI
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IViewManager
    {
        public interface IViewInstance : IDisposable
        {
            public ViewStatus CurrentStatus { get; }

            public IViewInstance BindModel(object model);

            public void Stack();

            public void Float();

            public void Detach();

            public void Hide();
        }

        public IViewInstance CurrentView { get; }

        public IViewInstance GetView<TView, TPresenter>(TView view, Func<TPresenter> presenterFactory)
            where TView : Component, IView
            where TPresenter : IPresenter;

        public UniTask<IViewInstance> GetView<TView, TPresenter>(string key, Func<TPresenter> presenterFactory)
            where TView : Component, IView
            where TPresenter : IPresenter;

        public UniTask<IViewInstance> GetView<TView, TPresenter>(Func<TPresenter> presenterFactory)
            where TView : Component, IView
            where TPresenter : IPresenter;

        public IViewInstance GetView<TView, TPresenter>(TView view)
            where TView : Component, IView
            where TPresenter : IPresenter, new();

        public UniTask<IViewInstance> GetView<TView, TPresenter>(string key)
            where TView : Component, IView
            where TPresenter : IPresenter, new();

        public UniTask<IViewInstance> GetView<TView, TPresenter>()
            where TView : Component, IView
            where TPresenter : IPresenter, new();
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