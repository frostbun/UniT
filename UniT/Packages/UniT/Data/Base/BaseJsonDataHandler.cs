namespace UniT.Data.Base
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using Unity.Plastic.Newtonsoft.Json;

    public abstract class BaseJsonDataHandler : IDataHandler
    {
        public async UniTask Populate(IData data)
        {
            var json = await this.GetJson(data.GetType());
            if (json.IsNullOrWhitespace()) return;
            JsonConvert.PopulateObject(json, data);
        }

        public async UniTask Save(IData data)
        {
            await this.SaveJson(JsonConvert.SerializeObject(data), data.GetType());
        }

        public abstract UniTask Flush();
        public abstract bool    CanHandle(Type type);

        protected abstract UniTask<string> GetJson(Type type);
        protected abstract UniTask         SaveJson(string json, Type type);
    }
}