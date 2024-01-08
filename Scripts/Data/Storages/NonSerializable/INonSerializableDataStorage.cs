namespace UniT.Data.Storages
{
    using UniT.Data.Types;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface INonSerializableDataStorage : IDataStorage
    {
        public IData[] Load(string[] keys);

        #if UNIT_UNITASK
        public UniTask<IData[]> LoadAsync(string[] keys, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif
    }
}