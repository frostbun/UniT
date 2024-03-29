namespace UniT.UI.Activity
{
    using Cysharp.Threading.Tasks;
    using UniT.UI.UIElement;

    public interface IActivity : IUIElement
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

        #if UNIT_UNITASK
        public UniTask<T> WaitForResult<T>();

        public UniTask WaitForHide();

        public bool SetResult(object result);
        #endif

        public void Hide(bool removeFromStack = true, bool autoStack = true);

        public void Dispose(bool autoStack = true);

        public bool IsDestroyed { get; }

        public T[] GetComponentsInChildren<T>();
    }

    public interface IActivityWithoutParams : IActivity, IUIElementWithoutParams
    {
        public IActivity Stack(bool force = false);

        public IActivity Float(bool force = false);

        public IActivity Dock(bool force = false);
    }

    public interface IActivityWithParams<TParams> : IActivity, IUIElementWithParams<TParams>
    {
        public IActivity Stack(TParams @params, bool force = true);

        public IActivity Float(TParams @params, bool force = true);

        public IActivity Dock(TParams @params, bool force = true);
    }
}