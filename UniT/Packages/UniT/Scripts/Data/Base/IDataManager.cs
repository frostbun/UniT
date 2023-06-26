namespace UniT.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;

    public interface IDataManager
    {
        public ILogger Logger { get; }

        public UniTask PopulateData(params Type[] dataTypes);

        public UniTask SaveData(params Type[] dataTypes);

        public UniTask FlushData(params Type[] dataTypes);

        public UniTask PopulateAllData();

        public UniTask SaveAllData();

        public UniTask FlushAllData();
    }
}