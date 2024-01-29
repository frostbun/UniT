namespace UniT.Data.Storages
{
    using System;
    using UniT.Data.Types;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public abstract class ReadWriteNonSerializableDataStorage : NonSerializableDataStorage, IReadWriteNonSerializableDataStorage
    {
        protected override bool CanStore(Type type) => base.CanStore(type) && typeof(IReadWriteData).IsAssignableFrom(type);

        void IReadWriteNonSerializableDataStorage.Save(string[] keys, IData[] values) => this.Save(keys, values);

        void IFlushableDataStorage.Flush() => this.Flush();

        protected abstract void Save(string[] keys, IData[] values);

        protected abstract void Flush();

        #if UNIT_UNITASK
        UniTask IReadWriteNonSerializableDataStorage.SaveAsync(string[] keys, IData[] values, IProgress<float> progress, CancellationToken cancellationToken) => this.SaveAsync(keys, values, progress, cancellationToken);

        UniTask IFlushableDataStorage.FlushAsync(IProgress<float> progress, CancellationToken cancellationToken) => this.FlushAsync(progress, cancellationToken);

        protected abstract UniTask SaveAsync(string[] keys, IData[] values, IProgress<float> progress, CancellationToken cancellationToken);

        protected abstract UniTask FlushAsync(IProgress<float> progress, CancellationToken cancellationToken);
        #else
        IEnumerator IReadWriteNonSerializableDataStorage.SaveAsync(string[] keys, IData[] values, Action callback, IProgress<float> progress) => this.SaveAsync(keys, values, callback, progress);

        IEnumerator IFlushableDataStorage.FlushAsync(Action callback, IProgress<float> progress) => this.FlushAsync(callback, progress);

        protected abstract IEnumerator SaveAsync(string[] keys, IData[] values, Action callback, IProgress<float> progress);

        protected abstract IEnumerator FlushAsync(Action callback, IProgress<float> progress);
        #endif
    }
}