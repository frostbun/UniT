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

    public interface INonSerializableDataStorage : IDataStorage
    {
        public IData[] Load(string[] keys);

        #if UNIT_UNITASK
        public UniTask<IData[]> LoadAsync(string[] keys, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator LoadAsync(string[] keys, Action<IData[]> callback, IProgress<float> progress = null);
        #endif
    }
}