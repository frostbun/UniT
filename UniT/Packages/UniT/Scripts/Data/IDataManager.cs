namespace UniT.Data
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;

    public interface IDataManager
    {
        public LogConfig LogConfig { get; }

        public T Get<T>() where T : IData;

        public UniTask Populate<T>() where T : IData;

        public UniTask Save<T>() where T : IData;

        public UniTask Flush<T>() where T : IData;

        public UniTask Populate(params Type[] dataTypes);

        public UniTask Save(params Type[] dataTypes);

        public UniTask Flush(params Type[] dataTypes);

        public UniTask PopulateAll();

        public UniTask SaveAll();

        public UniTask FlushAll();
    }
}