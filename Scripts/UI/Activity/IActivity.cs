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
            Stacking,
            Floating,
            Docked,
            Disposed,
        }

        public Status CurrentStatus { get; set; }

        public bool IsDestroyed { get; }

        public T[] GetComponentsInChildren<T>();

        #if UNIT_UNITASK
        public UniTask<T?> WaitForResult<T>();

        public UniTask WaitForHide();

        public bool SetResult(object result);
        #endif

        public void Hide(bool removeFromStack = true, bool autoStack = true);

        public void Dispose(bool autoStack = true);
    }

    public interface IActivityWithoutParams : IActivity, IViewWithoutParams
    {
        public IActivity Stack(bool force = false);

        public IActivity Float(bool force = false);

        public IActivity Dock(bool force = false);
    }

    public interface IActivityWithParams<TParams> : IActivity, IViewWithParams<TParams>
    {
        public IActivity Stack(TParams @params, bool force = true);

        public IActivity Float(TParams @params, bool force = true);

        public IActivity Dock(TParams @params, bool force = true);
    }
}