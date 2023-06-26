namespace UniT.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;

    public interface IDataManager
    {
        public ILogger Logger { get; }
        
        public UniTask PopulateData(Type type);

        public UniTask SaveData(Type type);

        public UniTask FlushHandler(Type type);

        public UniTask PopulateData<T>() where T : IData;

        public UniTask SaveData<T>() where T : IData;

        public UniTask FlushHandler<T>() where T : IDataHandler;

        public UniTask PopulateAllData();

        public UniTask SaveAllData();

        public UniTask FlushAllHandlers();
    }
}