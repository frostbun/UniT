#nullable enable
namespace UniT.UI.Activity
{
    using UniT.UI.View;
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #endif

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

        #if UNIT_UNITASK
        public UniTask<T> WaitForResult<T>();

        public UniTask WaitForHide();

        public bool SetResult(object? result);
        #endif

        public void Hide(bool autoStack = true);

        public void Dispose(bool autoStack = true);
    }

    public interface IActivityWithoutParams : IActivity, IViewWithoutParams
    {
        public IActivity Show(bool force = false);
    }

    public interface IActivityWithParams<TParams> : IActivity, IViewWithParams<TParams>
    {
        public IActivity Show(TParams @params, bool force = true);
    }
}