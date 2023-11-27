namespace UniT.Data.Storages
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IStorage
    {
        public bool CanStore(Type type);

        public string[] Load(string[] keys);

        #if UNIT_UNITASK
        public UniTask<string[]> LoadAsync(string[] keys, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif
    }
}