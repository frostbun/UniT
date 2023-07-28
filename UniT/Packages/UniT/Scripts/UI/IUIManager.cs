namespace UniT.UI
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UniT.UI.Interfaces;
    using UniT.UI.Item.Interfaces;
    using UnityEngine;

    public interface IUIManager
    {
        public LogConfig LogConfig { get; }

        public IView StackingView { get; }

        public IView NextStackingView { get; }

        public IEnumerable<IView> FloatingViews { get; }

        public IEnumerable<IView> DockedViews { get; }

        public UniTask<IView> GetView<TView>(string key) where TView : Component, IView;

        public UniTask<IView> GetView<TView>() where TView : Component, IView;

        public IView Initialize(IView view);

        public void Stack(IView view, bool force = false);

        public void Float(IView view, bool force = false);

        public void Dock(IView view, bool force = false);

        public void Hide(IView view, bool removeFromStack = true, bool autoStack = true);

        public void Dispose(IView view, bool autoStack = true);

        public IItemAdapter Initialize(IItemAdapter itemAdapter);
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