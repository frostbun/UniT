namespace UniT.Core.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;

    public abstract class BaseCsvDataHandler : IDataHandler
    {
        public UniTask Populate(IData data)
        {
            // TODO: Implement
            return UniTask.CompletedTask;
        }

        public UniTask Save(IData data)
        {
            // TODO: Implement
            return UniTask.CompletedTask;
        }

        public abstract UniTask Flush();

        public abstract bool CanHandle(Type type);
    }
}