namespace UniT.Data
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IWritableSerializableDataStorage : IWritableDataStorage
    {
        public void Save(string[] keys, string[] values);

        #if UNIT_UNITASK
        public UniTask SaveAsync(string[] keys, string[] values, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator SaveAsync(string[] keys, string[] values, Action callback = null, IProgress<float> progress = null);
        #endif
    }
}