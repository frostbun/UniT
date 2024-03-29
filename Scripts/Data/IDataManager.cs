namespace UniT.Data
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IDataManager
    {
        public IData Get(Type type);

        #if UNITY_2021_2_OR_NEWER
        public T Get<T>() where T : IData => (T)this.Get(typeof(T));
        #endif

        #region Sync

        public void Populate(params Type[] dataTypes);

        public void Save(params Type[] dataTypes);

        public void Flush(params Type[] dataTypes);

        public void PopulateAll();

        public void SaveAll();

        public void FlushAll();

        #if UNITY_2021_2_OR_NEWER
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

        #if UNITY_2021_2_OR_NEWER
        public UniTask PopulateAsync(Type dataType, IProgress<float> progress = null, CancellationToken cancellationToken = default) => this.PopulateAsync(new[] { dataType }, progress, cancellationToken);

        public UniTask SaveAsync(Type dataType, IProgress<float> progress = null, CancellationToken cancellationToken = default) => this.SaveAsync(new[] { dataType }, progress, cancellationToken);

        public UniTask FlushAsync(Type dataType, IProgress<float> progress = null, CancellationToken cancellationToken = default) => this.FlushAsync(new[] { dataType }, progress, cancellationToken);

        public UniTask PopulateAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : IData => this.PopulateAsync(typeof(T), progress, cancellationToken);

        public UniTask SaveAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : IReadWriteData => this.SaveAsync(typeof(T), progress, cancellationToken);

        public UniTask FlushAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : IReadWriteData => this.FlushAsync(typeof(T), progress, cancellationToken);
        #endif
        #else
        public IEnumerator PopulateAsync(Type[] dataTypes, Action callback = null, IProgress<float> progress = null);

        public IEnumerator SaveAsync(Type[] dataTypes, Action callback = null, IProgress<float> progress = null);

        public IEnumerator FlushAsync(Type[] dataTypes, Action callback = null, IProgress<float> progress = null);

        public IEnumerator PopulateAllAsync(Action callback = null, IProgress<float> progress = null);

        public IEnumerator SaveAllAsync(Action callback = null, IProgress<float> progress = null);

        public IEnumerator FlushAllAsync(Action callback = null, IProgress<float> progress = null);

        #if UNITY_2021_2_OR_NEWER
        public IEnumerator PopulateAsync(Type dataType, Action callback = null, IProgress<float> progress = null) => this.PopulateAsync(new[] { dataType }, callback, progress);

        public IEnumerator SaveAsync(Type dataType, Action callback = null, IProgress<float> progress = null) => this.SaveAsync(new[] { dataType }, callback, progress);

        public IEnumerator FlushAsync(Type dataType, Action callback = null, IProgress<float> progress = null) => this.FlushAsync(new[] { dataType }, callback, progress);

        public IEnumerator PopulateAsync<T>(Action callback = null, IProgress<float> progress = null) where T : IData => this.PopulateAsync(typeof(T), callback, progress);

        public IEnumerator SaveAsync<T>(Action callback = null, IProgress<float> progress = null) where T : IReadWriteData => this.SaveAsync(typeof(T), callback, progress);

        public IEnumerator FlushAsync<T>(Action callback = null, IProgress<float> progress = null) where T : IReadWriteData => this.FlushAsync(typeof(T), callback, progress);
        #endif
        #endif

        #endregion
    }
}