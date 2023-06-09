namespace UniT.Core.Data.Converters.Implements
{
    using System;
    using Newtonsoft.Json;
    using UniT.Core.Data.Converters.Base;

    public class DefaultConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(object);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return JsonConvert.DeserializeObject(str, type);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}