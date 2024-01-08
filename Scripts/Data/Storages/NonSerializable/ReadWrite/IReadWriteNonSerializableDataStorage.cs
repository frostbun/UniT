namespace UniT.Data.Storages
{
    using UniT.Data.Types;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IReadWriteNonSerializableDataStorage : INonSerializableDataStorage, IFlushableDataStorage
    {
        public void Save(string[] keys, IData[] values);

        #if UNIT_UNITASK
        public UniTask SaveAsync(string[] keys, IData[] values, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif
    }
}