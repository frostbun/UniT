namespace UniT.Data.Storages
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public abstract class ReadWriteStorage : Storage, IReadWriteStorage
    {
        protected override bool CanStore(Type type) => typeof(IReadWriteData).IsAssignableFrom(type);

        void IReadWriteStorage.Save(string[] keys, string[] values) => this.Save(keys, values);

        void IReadWriteStorage.Flush() => this.Flush();

        protected abstract void Save(string[] keys, string[] values);

        protected abstract void Flush();

        #if UNIT_UNITASK
        UniTask IReadWriteStorage.SaveAsync(string[] keys, string[] values, IProgress<float> progress, CancellationToken cancellationToken) => this.SaveAsync(keys, values, progress, cancellationToken);

        UniTask IReadWriteStorage.FlushAsync(IProgress<float> progress, CancellationToken cancellationToken) => this.FlushAsync(progress, cancellationToken);

        protected abstract UniTask SaveAsync(string[] keys, string[] values, IProgress<float> progress, CancellationToken cancellationToken);

        protected abstract UniTask FlushAsync(IProgress<float> progress, CancellationToken cancellationToken);
        #endif
    }
}