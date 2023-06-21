namespace UniT.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IDataHandler
    {
        protected internal bool CanHandle(Type type);

        protected internal UniTask Populate(IData data);

        protected internal UniTask Save(IData data);

        protected internal UniTask Flush();
    }
}