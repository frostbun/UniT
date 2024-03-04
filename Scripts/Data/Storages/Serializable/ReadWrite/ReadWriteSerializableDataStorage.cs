namespace UniT.Data
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public abstract class ReadWriteSerializableDataStorage : SerializableDataStorage, IReadWriteSerializableDataStorage
    {
        protected override bool CanStore(Type type) => base.CanStore(type) && typeof(IReadWriteData).IsAssignableFrom(type);

        void IReadWriteSerializableDataStorage.Save(string[] keys, string[] values) => this.Save(keys, values);

        void IFlushableDataStorage.Flush() => this.Flush();

        protected abstract void Save(string[] keys, string[] values);

        protected abstract void Flush();

        #if UNIT_UNITASK
        UniTask IReadWriteSerializableDataStorage.SaveAsync(string[] keys, string[] values, IProgress<float> progress, CancellationToken cancellationToken) => this.SaveAsync(keys, values, progress, cancellationToken);

        UniTask IFlushableDataStorage.FlushAsync(IProgress<float> progress, CancellationToken cancellationToken) => this.FlushAsync(progress, cancellationToken);

        protected abstract UniTask SaveAsync(string[] keys, string[] values, IProgress<float> progress, CancellationToken cancellationToken);

        protected abstract UniTask FlushAsync(IProgress<float> progress, CancellationToken cancellationToken);
        #else
        IEnumerator IReadWriteSerializableDataStorage.SaveAsync(string[] keys, string[] values, Action callback, IProgress<float> progress) => this.SaveAsync(keys, values, callback, progress);

        IEnumerator IFlushableDataStorage.FlushAsync(Action callback, IProgress<float> progress) => this.FlushAsync(callback, progress);

        protected abstract IEnumerator SaveAsync(string[] keys, string[] values, Action callback, IProgress<float> progress);

        protected abstract IEnumerator FlushAsync(Action callback, IProgress<float> progress);
        #endif
    }
}