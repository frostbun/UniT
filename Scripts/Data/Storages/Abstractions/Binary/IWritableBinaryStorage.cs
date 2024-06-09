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

    public interface IWritableBinaryStorage : IWritableDataStorage
    {
        public void Write(string[] keys, byte[][] values);

        #if UNIT_UNITASK
        public UniTask WriteAsync(string[] keys, byte[][] values, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator WriteAsync(string[] keys, byte[][] values, Action? callback = null, IProgress<float>? progress = null);
        #endif
    }
}