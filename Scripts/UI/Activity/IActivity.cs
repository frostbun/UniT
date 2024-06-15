#nullable enable
namespace UniT.UI.Activity
{
    using UniT.UI.View;

    public interface IActivity : IView
    {
        public enum Status
        {
            Hidden,
            Showing,
            Disposed,
        }

        public Status CurrentStatus { get; set; }

        public bool IsDestroyed { get; }

        public T[] GetComponentsInChildren<T>();

        public void Hide(bool autoStack = true);

        public void Dispose(bool autoStack = true);
    }

    public interface IActivityWithoutParams : IActivity, IViewWithoutParams
    {
        public void Show(bool force = false);
    }

    public interface IActivityWithParams<TParams> : IActivity, IViewWithParams<TParams>
    {
        public void Show(TParams @params, bool force = true);
    }
}