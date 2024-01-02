namespace UniT.Data.Storages
{
    using System;
    using UniT.Data.Types;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public abstract class ReadWriteBlobDataStorage : BlobDataStorage, IReadWriteBlobDataStorage
    {
        protected override bool CanStore(Type type) => base.CanStore(type) && typeof(IReadWriteData).IsAssignableFrom(type);

        void IReadWriteBlobDataStorage.Save(string[] keys, IData[] values) => this.Save(keys, values);

        void IFlushableDataStorage.Flush() => this.Flush();

        protected abstract void Save(string[] keys, IData[] values);

        protected abstract void Flush();

        #if UNIT_UNITASK
        UniTask IReadWriteBlobDataStorage.SaveAsync(string[] keys, IData[] values, IProgress<float> progress, CancellationToken cancellationToken) => this.SaveAsync(keys, values, progress, cancellationToken);

        UniTask IFlushableDataStorage.FlushAsync(IProgress<float> progress, CancellationToken cancellationToken) => this.FlushAsync(progress, cancellationToken);

        protected abstract UniTask SaveAsync(string[] keys, IData[] values, IProgress<float> progress, CancellationToken cancellationToken);

        protected abstract UniTask FlushAsync(IProgress<float> progress, CancellationToken cancellationToken);
        #endif
    }
}