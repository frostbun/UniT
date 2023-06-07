namespace UniT.Core.Converters.Implement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Core.Converters.Base;

    public class DictionaryGenericConverter : ListGenericConverter
    {
        private readonly string separator;

        public DictionaryGenericConverter(string separator = ":", string listSeparator = ";") : base(listSeparator)
        {
            this.separator = separator;
        }

        protected override Type ConvertibleType => typeof(Dictionary<,>);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            var keyType        = type.GetGenericArguments()[0];
            var valueType      = type.GetGenericArguments()[1];
            var keyConverter   = ConverterManager.Instance.GetConverter(keyType);
            var valueConverter = ConverterManager.Instance.GetConverter(valueType);
            var dict           = (IDictionary)Activator.CreateInstance(type);
            foreach (var item in (List<string>)base.ConvertFromString_Internal(str, typeof(List<string>)))
            {
                var kv = item.Split(this.separator);
                dict.Add(keyConverter.ConvertFromString(kv[0], keyType), valueConverter.ConvertFromString(kv[1], valueType));
            }

            return dict;
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            var keyType        = type.GetGenericArguments()[0];
            var valueType      = type.GetGenericArguments()[1];
            var keyConverter   = ConverterManager.Instance.GetConverter(keyType);
            var valueConverter = ConverterManager.Instance.GetConverter(valueType);
            return base.ConvertToString_Internal(((IDictionary)obj).Cast<DictionaryEntry>().Select(kv => $"{keyConverter.ConvertToString(kv.Key, keyType)}{this.separator}{valueConverter.ConvertToString(kv.Value, valueType)}").ToList(), typeof(List<string>));
        }
    }
}