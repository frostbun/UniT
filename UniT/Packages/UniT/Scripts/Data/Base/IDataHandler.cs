namespace UniT.Data.Base
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public interface IDataHandler
    {
        protected internal bool CanHandle(Type type);

        protected internal UniTask Populate(IData[] datas);

        protected internal UniTask Save(IData[] datas);

        protected internal UniTask Flush();
    }
}