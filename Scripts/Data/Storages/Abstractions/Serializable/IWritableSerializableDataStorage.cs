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

    public interface IWritableSerializableDataStorage : IWritableDataStorage
    {
        public void WriteStrings(string[] keys, string[] values);

        public void WriteBytes(string[] keys, byte[][] values);

        #if UNIT_UNITASK
        public UniTask WriteStringsAsync(string[] keys, string[] values, IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask WriteBytesAsync(string[] keys, byte[][] values, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator WriteStringsAsync(string[] keys, string[] values, Action? callback = null, IProgress<float>? progress = null);

        public IEnumerator WriteBytesAsync(string[] keys, byte[][] values, Action? callback = null, IProgress<float>? progress = null);
        #endif
    }
}