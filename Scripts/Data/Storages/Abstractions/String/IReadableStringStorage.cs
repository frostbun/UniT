#nullable enable
namespace UniT.Data.Storage
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IReadableStringStorage : IReadableDataStorage
    {
        public string[] Read(string[] keys);

        #if UNIT_UNITASK
        public UniTask<string[]> ReadAsync(string[] keys, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator ReadAsync(string[] keys, Action<string[]> callback, IProgress<float>? progress = null);
        #endif
    }
}