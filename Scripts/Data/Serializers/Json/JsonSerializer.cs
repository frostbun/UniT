#if UNIT_NEWTONSOFT_JSON
namespace UniT.Data
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    public sealed class JsonSerializer : IStringSerializer
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

        void IStringSerializer.Populate(IData data, string rawData)
        {
            JsonConvert.PopulateObject(rawData, data, this.settings);
        }

        string IStringSerializer.Serialize(IData data)
        {
            return JsonConvert.SerializeObject(data, this.settings);
        }
    }
}
#endif