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

    public interface IWritableDataStorage : IDataStorage
    {
        public void Flush();

        #if UNIT_UNITASK
        public UniTask FlushAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator FlushAsync(Action? callback = null, IProgress<float>? progress = null);
        #endif
    }
}