namespace UniT.Data.Storages
{
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IReadWriteSerializableDataStorage : ISerializableDataStorage, IFlushableDataStorage
    {
        public void Save(string[] keys, string[] values);

        #if UNIT_UNITASK
        public UniTask SaveAsync(string[] keys, string[] values, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif
    }
}