namespace UniT.UI
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.UI.Interfaces;
    using UniT.UI.Item.Interfaces;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public interface IUIManager
    {
        #region Public APIs

        public ILogger Logger { get; }

        public IView StackingView { get; }

        public IView NextStackingView { get; }

        public IEnumerable<IView> FloatingViews { get; }

        public IEnumerable<IView> DockedViews { get; }

        public UniTask<IView> GetView<TView>(string key) where TView : Component, IView;

        public UniTask<IView> GetView<TView>() where TView : Component, IView;

        public IView Initialize(IView view);

        public IItemAdapter Initialize(IItemAdapter itemAdapter);

        #endregion

        #region Internal APIs

        protected internal void Stack(IView view);

        protected internal void Float(IView view);

        protected internal void Dock(IView view);

        protected internal void Hide(IView view);

        protected internal void Dispose(IView view);

        protected internal void RemoveFromStack(IView view);

        protected internal void StackNextView();

        #endregion
    }

    public static class ViewExtensions
    {
        public static UniTask<IView> PutExtra<T>(this UniTask<IView> task, string key, T value)
        {
            return task.ContinueWith(view => view.PutExtra(key, value));
        }

        public static UniTask Stack(this UniTask<IView> task, bool force = false)
        {
            return task.ContinueWith(view => view.Stack(force));
        }

        public static UniTask Float(this UniTask<IView> task, bool force = false)
        {
            return task.ContinueWith(view => view.Float(force));
        }

        public static UniTask Dock(this UniTask<IView> task, bool force = false)
        {
            return task.ContinueWith(view => view.Dock(force));
        }

        public static UniTask Hide(this UniTask<IView> task, bool removeFromStack = true, bool autoStack = true)
        {
            return task.ContinueWith(view => view.Hide(removeFromStack, autoStack));
        }

        public static UniTask Dispose(this UniTask<IView> task, bool autoStack = true)
        {
            return task.ContinueWith(view => view.Dispose(autoStack));
        }
    }
}