namespace UniT.Data.Storages
{
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IFlushableDataStorage
    {
        public void Flush();

        #if UNIT_UNITASK
        public UniTask FlushAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif
    }
}