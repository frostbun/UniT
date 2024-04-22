namespace UniT.Initializables
{
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IAsyncEarlyInitializable
    {
        #if UNIT_UNITASK
        public UniTask InitializeAsync();
        #else
        public IEnumerator InitializeAsync();
        #endif
    }

    public interface IAsyncInitializable
    {
        #if UNIT_UNITASK
        public UniTask InitializeAsync();
        #else
        public IEnumerator InitializeAsync();
        #endif
    }

    public interface IAsyncLateInitializable
    {
        #if UNIT_UNITASK
        public UniTask InitializeAsync();
        #else
        public IEnumerator InitializeAsync();
        #endif
    }
}