#if UNIT_NEWTONSOFT_JSON
namespace UniT.Data
{
    using System;
    using Newtonsoft.Json;

    public sealed class JsonConverter : Converter<object>
    {
        private readonly JsonSerializerSettings settings;

        public JsonConverter(JsonSerializerSettings settings = null)
        {
            this.settings = settings
                ?? new JsonSerializerSettings
                {
                    TypeNameHandling       = TypeNameHandling.Auto,
                    ReferenceLoopHandling  = ReferenceLoopHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                };
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return JsonConvert.DeserializeObject(str, type, this.settings);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, this.settings);
        }
    }
}
#endif