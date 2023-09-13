namespace UniT
{
    using Cysharp.Threading.Tasks;

    public interface IInitializable
    {
        public void Initialize();
    }

    public interface IEarlyInitializable
    {
        public void Initialize();
    }

    public interface ILateInitializable
    {
        public void Initialize();
    }

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