#nullable enable
namespace UniT.Services
{
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;

    public interface IAsyncEarlyInitializable
    {
        public UniTask InitializeAsync();
    }

    public interface IAsyncInitializable
    {
        public UniTask InitializeAsync();
    }

    public interface IAsyncLateInitializable
    {
        public UniTask InitializeAsync();
    }
    #else
    using System.Collections;

    public interface IAsyncEarlyInitializable
    {
        public IEnumerator InitializeAsync();
    }

    public interface IAsyncInitializable
    {
        public IEnumerator InitializeAsync();
    }

    public interface IAsyncLateInitializable
    {
        public IEnumerator InitializeAsync();
    }
    #endif
}