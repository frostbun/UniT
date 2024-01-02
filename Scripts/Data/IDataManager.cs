namespace UniT.Data
{
    using System;
    using UniT.Data.Types;
    using UniT.Logging;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IDataManager : IHasLogger
    {
        public IData Get(Type type);

        #if NET_STANDARD_2_1
        public T Get<T>() where T : IData => (T)this.Get(typeof(T));
        #endif

        #region Sync

        public void Populate(params Type[] dataTypes);

        public void Save(params Type[] dataTypes);

        public void Flush(params Type[] dataTypes);

        public void PopulateAll();

        public void SaveAll();

        public void FlushAll();

        #if NET_STANDARD_2_1
        public void Populate<T>() where T : IData => this.Populate(typeof(T));

        public void Save<T>() where T : IReadWriteData => this.Save(typeof(T));

        public void Flush<T>() where T : IReadWriteData => this.Flush(typeof(T));
        #endif

        #endregion

        #region Async

        #if UNIT_UNITASK
        public UniTask PopulateAsync(Type[] dataTypes, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask SaveAsync(Type[] dataTypes, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask FlushAsync(Type[] dataTypes, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask PopulateAllAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask SaveAllAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask FlushAllAsync(IProgress<float> progress = null, CancellationToken cancellationToken = default);

        #if NET_STANDARD_2_1
        public UniTask PopulateAsync(Type dataType, IProgress<float> progress = null, CancellationToken cancellationToken = default) => this.PopulateAsync(new[] { dataType }, progress, cancellationToken);

        public UniTask SaveAsync(Type dataType, IProgress<float> progress = null, CancellationToken cancellationToken = default) => this.SaveAsync(new[] { dataType }, progress, cancellationToken);

        public UniTask FlushAsync(Type dataType, IProgress<float> progress = null, CancellationToken cancellationToken = default) => this.FlushAsync(new[] { dataType }, progress, cancellationToken);

        public UniTask PopulateAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : IData => this.PopulateAsync(typeof(T), progress, cancellationToken);

        public UniTask SaveAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : IReadWriteData => this.SaveAsync(typeof(T), progress, cancellationToken);

        public UniTask FlushAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : IReadWriteData => this.FlushAsync(typeof(T), progress, cancellationToken);
        #endif
        #endif

        #endregion
    }
}