namespace UniT.Core.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;

    public abstract class BaseCsvDataHandler : IDataHandler
    {
        public UniTask Populate(IData data)
        {
            throw new NotImplementedException();
        }

        public UniTask Save(IData data)
        {
            throw new NotImplementedException();
        }

        public abstract UniTask Flush();

        public abstract bool CanHandle(Type type);
    }
}