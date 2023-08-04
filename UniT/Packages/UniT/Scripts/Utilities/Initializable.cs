namespace UniT.Utilities
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;

    public interface IInitializable
    {
        public void Initialize();
    }

    public interface IAsyncInitializable
    {
        public UniTask InitializeAsync();
    }

    public static class InitializableExtensions
    {
        public static void InitializeAll(this IEnumerable<IInitializable> initializables) => initializables.ForEach(initializable => initializable.Initialize());

        public static UniTask InitializeAllAsync(this IEnumerable<IAsyncInitializable> asyncInitializables) => UniTask.WhenAll(asyncInitializables.Select(asyncInitializable => asyncInitializable.InitializeAsync()));
    }
}