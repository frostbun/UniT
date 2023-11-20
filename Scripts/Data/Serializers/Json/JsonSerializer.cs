namespace UniT.Data.Serializers
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    public sealed class JsonSerializer : ISerializer
    {
        [Preserve]
        public JsonSerializer()
        {
        }

        public bool CanSerialize(Type type)
        {
            return typeof(IJsonData).IsAssignableFrom(type);
        }

        public void Populate(object data, string rawData)
        {
            JsonConvert.PopulateObject(rawData, data);
        }

        public string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}