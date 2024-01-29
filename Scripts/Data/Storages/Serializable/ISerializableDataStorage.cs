namespace UniT.Data.Storages
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface ISerializableDataStorage : IDataStorage
    {
        public string[] Load(string[] keys);

        #if UNIT_UNITASK
        public UniTask<string[]> LoadAsync(string[] keys, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator LoadAsync(string[] keys, Action<string[]> callback, IProgress<float> progress = null);
        #endif
    }
}