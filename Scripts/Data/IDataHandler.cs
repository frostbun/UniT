namespace UniT.Data
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;

    public interface IDataHandler
    {
        public LogConfig LogConfig { get; }

        protected internal bool CanHandle(Type type);

        protected internal UniTask Populate(IData[] datas);

        protected internal UniTask Save(IData[] datas);

        protected internal UniTask Flush();
    }
}