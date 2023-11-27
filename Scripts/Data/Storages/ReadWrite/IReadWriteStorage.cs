namespace UniT.Data.Storages
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IReadWriteStorage : IStorage
    {
        public void Save(string[] keys, string[] values);

        public void Flush();

        #if UNIT_UNITASK
        public UniTask SaveAsync(string[] keys, string[] values, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask FlushAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif
    }
}