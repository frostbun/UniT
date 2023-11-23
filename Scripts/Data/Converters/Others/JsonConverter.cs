namespace UniT.Data.Converters
{
    using System;
    using Newtonsoft.Json;

    public sealed class JsonConverter : BaseConverter
    {
        protected override Type ConvertibleType { get; } = typeof(object);

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