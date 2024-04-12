#if UNIT_NEWTONSOFT_JSON
namespace UniT.Data
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    public sealed class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings settings;

        [Preserve]
        public JsonSerializer(JsonSerializerSettings settings = null)
        {
            this.settings = settings
                ?? new JsonSerializerSettings
                {
                    TypeNameHandling       = TypeNameHandling.Auto,
                    ReferenceLoopHandling  = ReferenceLoopHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                };
        }

        bool ISerializer.CanSerialize(Type type) => typeof(IJsonData).IsAssignableFrom(type);

        void ISerializer.Populate(IData data, string rawData)
        {
            JsonConvert.PopulateObject(rawData, data, this.settings);
        }

        string ISerializer.Serialize(IData data)
        {
            return JsonConvert.SerializeObject(data, this.settings);
        }
    }
}
#endif