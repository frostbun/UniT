namespace UniT.Core.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IDataManager
    {
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