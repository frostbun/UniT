namespace UniT.Core.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using UniT.Core.Extensions;

    public abstract class BaseJsonDataHandler : IDataHandler
    {
        public UniTask Populate(IData data)
        {
            return this.GetJson(data.GetType().GetKeyAttribute())
                       .ContinueWith(json =>
                       {
                           if (json.IsNullOrWhitespace()) return;
                           JsonConvert.PopulateObject(json, data);
                       });
        }

        public UniTask Save(IData data)
        {
            return this.SaveJson(data.GetType().GetKeyAttribute(), JsonConvert.SerializeObject(data));
        }

        public abstract UniTask Flush();
        public abstract bool    CanHandle(Type type);

        protected abstract UniTask<string> GetJson(string key);
        protected abstract UniTask         SaveJson(string key, string json);
    }
}