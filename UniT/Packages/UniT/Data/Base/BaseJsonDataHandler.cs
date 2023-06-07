namespace UniT.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using Newtonsoft.Json;

    public abstract class BaseJsonDataHandler : IDataHandler
    {
        public UniTask Populate(IData data)
        {
            return this.GetJson(data.GetType())
                       .ContinueWith(json =>
                       {
                           if (json.IsNullOrWhitespace()) return;
                           JsonConvert.PopulateObject(json, data);
                       });
        }

        public UniTask Save(IData data)
        {
            return this.SaveJson(JsonConvert.SerializeObject(data), data.GetType());
        }

        public abstract UniTask Flush();
        public abstract bool    CanHandle(Type type);

        protected abstract UniTask<string> GetJson(Type type);
        protected abstract UniTask         SaveJson(string json, Type type);
    }
}