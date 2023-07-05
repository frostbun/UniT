namespace UniT.Utilities
{
    using Cysharp.Threading.Tasks;

    public interface IInitializable
    {
        public void Initialize();
    }

    public interface IAsyncInitializable
    {
        public UniTask InitializeAsync();
    }
}