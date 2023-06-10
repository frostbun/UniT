namespace UniT.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;

    public abstract class BaseDataHandler : IDataHandler
    {
        public virtual bool CanHandle(Type type)
        {
            return typeof(IData).IsAssignableFrom(type);
        }

        public UniTask Populate(IData data)
        {
            return this.GetRawData_Internal(data.GetType().GetKeyAttribute())
                       .ContinueWith(rawData =>
                       {
                           if (rawData.IsNullOrWhitespace()) return;
                           this.PopulateData_Internal(rawData, data);
                       });
        }

        public UniTask Save(IData data)
        {
            return this.SaveRawData_Internal(data.GetType().GetKeyAttribute(), this.SerializeData_Internal(data));
        }

        public abstract UniTask Flush();

        protected abstract UniTask<string> GetRawData_Internal(string key);
        protected abstract UniTask         SaveRawData_Internal(string key, string rawData);
        protected abstract void            PopulateData_Internal(string rawData, IData data);
        protected abstract string          SerializeData_Internal(IData data);
    }
}