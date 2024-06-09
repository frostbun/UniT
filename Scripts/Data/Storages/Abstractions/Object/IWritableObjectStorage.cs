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

    public interface IWritableObjectStorage : IWritableDataStorage
    {
        public void Write(string[] keys, object[] values);

        #if UNIT_UNITASK
        public UniTask WriteAsync(string[] keys, object[] values, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator WriteAsync(string[] keys, object[] values, Action? callback = null, IProgress<float>? progress = null);
        #endif
    }
}