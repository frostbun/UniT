namespace UniT.Data
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;

    public interface IDataHandler
    {
        public LogConfig LogConfig { get; }

        public bool CanHandle(Type type);

        public UniTask Populate(IData[] datas);

        public UniTask Save(IData[] datas);

        public UniTask Flush();
    }
}