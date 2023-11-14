namespace UniT.Data
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Data.Storages;
    using UniT.Logging;

    public interface IDataManager
    {
        public LogConfig LogConfig { get; }

        public IReadOnlyData Get(Type type);

        public UniTask Populate(params Type[] dataTypes);

        public UniTask Save(params Type[] dataTypes);

        public UniTask Flush(params Type[] dataTypes);

        public UniTask PopulateAll();

        public UniTask SaveAll();

        public UniTask FlushAll();

        #region Generic

        public T Get<T>() where T : IReadOnlyData => (T)this.Get(typeof(T));

        public UniTask Populate<T>() where T : IReadOnlyData => this.Populate(typeof(T));

        public UniTask Save<T>() where T : IData => this.Save(typeof(T));

        public UniTask Flush<T>() where T : IData => this.Flush(typeof(T));

        #endregion
    }
}