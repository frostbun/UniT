#nullable enable
namespace UniT.Services
{
    using System;
    using System.Linq;
    using UniT.Extensions;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class InitializableManager
    {
        private readonly IEarlyInitializable[]      earlyInitializables;
        private readonly IAsyncEarlyInitializable[] asyncEarlyInitializables;
        private readonly IInitializable[]           initializables;
        private readonly IAsyncInitializable[]      asyncInitializables;
        private readonly ILateInitializable[]       lateInitializables;
        private readonly IAsyncLateInitializable[]  asyncLateInitializables;

        public InitializableManager(
            IEarlyInitializable[]      earlyInitializables,
            IAsyncEarlyInitializable[] asyncEarlyInitializables,
            IInitializable[]           initializables,
            IAsyncInitializable[]      asyncInitializables,
            ILateInitializable[]       lateInitializables,
            IAsyncLateInitializable[]  asyncLateInitializables
        )
        {
            this.earlyInitializables      = earlyInitializables;
            this.asyncEarlyInitializables = asyncEarlyInitializables;
            this.initializables           = initializables;
            this.asyncInitializables      = asyncInitializables;
            this.lateInitializables       = lateInitializables;
            this.asyncLateInitializables  = asyncLateInitializables;
        }

        #if UNIT_UNITASK
        public async UniTask InitializeAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default)
        {
            var subProgresses = progress.CreateSubProgresses(3).ToArray();
            this.earlyInitializables.ForEach(service => service.Initialize());
            await this.asyncEarlyInitializables.ForEachAsync(
                (service, progress, cancellationToken) => service.InitializeAsync(progress, cancellationToken),
                subProgresses[0],
                cancellationToken
            );
            subProgresses[0]?.Report(1);
            this.initializables.ForEach(service => service.Initialize());
            await this.asyncInitializables.ForEachAsync(
                (service, progress, cancellationToken) => service.InitializeAsync(progress, cancellationToken),
                subProgresses[1],
                cancellationToken
            );
            subProgresses[1]?.Report(1);
            this.lateInitializables.ForEach(service => service.Initialize());
            await this.asyncLateInitializables.ForEachAsync(
                (service, progress, cancellationToken) => service.InitializeAsync(progress, cancellationToken),
                subProgresses[2],
                cancellationToken
            );
            subProgresses[2]?.Report(1);
        }
        #else
        public IEnumerator InitializeAsync(Action? callback = null, IProgress<float>? progress = null)
        {
            var subProgresses = progress.CreateSubProgresses(3).ToArray();
            this.earlyInitializables.ForEach(service => service.Initialize());
            yield return this.asyncEarlyInitializables.ForEachAsync(
                (service, progress) => service.InitializeAsync(progress),
                progress: subProgresses[0]
            );
            subProgresses[0]?.Report(1);
            this.initializables.ForEach(service => service.Initialize());
            yield return this.asyncInitializables.ForEachAsync(
                (service, progress) => service.InitializeAsync(progress),
                progress: subProgresses[1]
            );
            subProgresses[1]?.Report(1);
            this.lateInitializables.ForEach(service => service.Initialize());
            yield return this.asyncLateInitializables.ForEachAsync(
                (service, progress) => service.InitializeAsync(progress),
                progress: subProgresses[2]
            );
            subProgresses[2]?.Report(1);
            callback?.Invoke();
        }
        #endif
    }
}