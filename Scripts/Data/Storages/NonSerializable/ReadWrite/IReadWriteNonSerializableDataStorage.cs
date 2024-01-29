namespace UniT.Data.Storages
{
    using System;
    using UniT.Data.Types;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IReadWriteNonSerializableDataStorage : INonSerializableDataStorage, IFlushableDataStorage
    {
        public void Save(string[] keys, IData[] values);

        #if UNIT_UNITASK
        public UniTask SaveAsync(string[] keys, IData[] values, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator SaveAsync(string[] keys, IData[] values, Action callback = null, IProgress<float> progress = null);
        #endif
    }
}