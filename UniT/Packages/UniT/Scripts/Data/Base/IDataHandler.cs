namespace UniT.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IDataHandler
    {
        public bool CanHandle(Type type);

        public UniTask Populate(IData data);

        public UniTask Save(IData data);

        public UniTask Flush();
    }
}