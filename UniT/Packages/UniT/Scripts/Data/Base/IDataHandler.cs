namespace UniT.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;

    public interface IDataHandler
    {
        public ILogger Logger { get; }

        protected internal bool CanHandle(Type type);

        protected internal UniTask Populate(IData[] datas);

        protected internal UniTask Save(IData[] datas);

        protected internal UniTask Flush();
    }
}