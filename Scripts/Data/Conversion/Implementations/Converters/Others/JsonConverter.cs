#if UNIT_JSON
#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    public sealed class JsonConverter : Converter<object>
    {
        private readonly JsonSerializerSettings settings;

        [Preserve]
        public JsonConverter(JsonSerializerSettings? settings = null)
        {
            this.settings = settings
                ?? new JsonSerializerSettings
                {
                    TypeNameHandling      = TypeNameHandling.Auto,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return JsonConvert.DeserializeObject(str, type, this.settings)!;
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, this.settings);
        }
    }
}
#endif