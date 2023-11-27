#if UNIT_NEWTONSOFT_JSON
namespace UniT.Data.Converters
{
    using System;
    using Newtonsoft.Json;

    public sealed class JsonConverter : Converter<object>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return JsonConvert.DeserializeObject(str, type);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
#endif