#nullable enable
namespace UniT.Services
{
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public interface IAsyncEarlyInitializable
    {
        public UniTask InitializeAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncInitializable
    {
        public UniTask InitializeAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncLateInitializable
    {
        public UniTask InitializeAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
    }
    #else
    using System;
    using System.Collections;

    public interface IAsyncEarlyInitializable
    {
        public IEnumerator InitializeAsync(IProgress<float>? progress = null);
    }

    public interface IAsyncInitializable
    {
        public IEnumerator InitializeAsync(IProgress<float>? progress = null);
    }

    public interface IAsyncLateInitializable
    {
        public IEnumerator InitializeAsync(IProgress<float>? progress = null);
    }
    #endif
}