namespace UniT.Initializables
{
    using Cysharp.Threading.Tasks;

    public interface IAsyncInitializable
    {
        public UniTask InitializeAsync();
    }

    public interface IAsyncEarlyInitializable
    {
        public UniTask InitializeAsync();
    }

    public interface IAsyncLateInitializable
    {
        public UniTask InitializeAsync();
    }
}