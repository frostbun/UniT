#if UNIT_UNITASK
namespace UniT.Factories
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public interface IAsyncFactory<TProduct>
    {
        public UniTask<TProduct> CreateAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam0, TProduct>
    {
        public UniTask<TProduct> CreateAsync(TParam0 param0, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam0, in TParam1, TProduct>
    {
        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam0, in TParam1, in TParam2, TProduct>
    {
        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam0, in TParam1, in TParam2, in TParam3, TProduct>
    {
        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, TProduct>
    {
        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, TProduct>
    {
        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, TProduct>
    {
        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, TProduct>
    {
        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, in TParam8, TProduct>
    {
        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }

    public interface IAsyncFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, in TParam8, in TParam9, TProduct>
    {
        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }
}
#endif