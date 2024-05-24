#nullable enable
namespace UniT.Data
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IReadableSerializableDataStorage : IReadableDataStorage
    {
        public string[] ReadStrings(string[] keys);

        public byte[][] ReadBytes(string[] keys);

        #if UNIT_UNITASK
        public UniTask<string[]> ReadStringsAsync(string[] keys, IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask<byte[][]> ReadBytesAsync(string[] keys, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator ReadStringsAsync(string[] keys, Action<string[]> callback, IProgress<float>? progress = null);

        public IEnumerator ReadBytesAsync(string[] keys, Action<byte[][]> callback, IProgress<float>? progress = null);
        #endif
    }
}