namespace UniT.Data.Converters.Base
{
    using System;
    using Newtonsoft.Json;

    public class JsonConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(object);

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