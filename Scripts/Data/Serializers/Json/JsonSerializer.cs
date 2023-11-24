namespace UniT.Data.Serializers
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
                    TypeNameHandling      = TypeNameHandling.Auto,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };
        }

        public bool CanSerialize(Type type)
        {
            return typeof(IJsonData).IsAssignableFrom(type);
        }

        public void Populate(object data, string rawData)
        {
            JsonConvert.PopulateObject(rawData, data, this.settings);
        }

        public string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, this.settings);
        }
    }
}