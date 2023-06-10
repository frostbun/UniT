namespace UniT.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IDataHandler
    {
        public UniTask Populate(IData data);

        public UniTask Save(IData data);

        public UniTask Flush();

        public bool CanHandle(Type type);
    }
}