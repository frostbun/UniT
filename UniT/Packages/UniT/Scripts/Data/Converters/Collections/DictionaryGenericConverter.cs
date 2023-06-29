namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Data.Converters.Base;
    using UniT.Extensions;

    public class DictionaryGenericConverter : BaseGenericConverter
    {
        private readonly string separator;

        public DictionaryGenericConverter(string separator = ":")
        {
            this.separator = separator;
        }

        protected override Type ConvertibleType => typeof(Dictionary<,>);

        protected override object ConvertFromString(string str, Type type)
        {
            var keyType        = type.GetGenericArguments()[0];
            var valueType      = type.GetGenericArguments()[1];
            var arrayType      = typeof(string[]);
            var keyConverter   = ConverterManager.Instance.GetConverter(keyType);
            var valueConverter = ConverterManager.Instance.GetConverter(valueType);
            var arrayConverter = ConverterManager.Instance.GetConverter(arrayType);
            var dictionary     = (IDictionary)Activator.CreateInstance(type);
            foreach (var item in (string[])arrayConverter.ConvertFromString(str, arrayType))
            {
                var kv = item.Split(this.separator);
                dictionary.Add(keyConverter.ConvertFromString(kv[0], keyType), valueConverter.ConvertFromString(kv[1], valueType));
            }

            return dictionary;
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var keyType        = type.GetGenericArguments()[0];
            var valueType      = type.GetGenericArguments()[1];
            var arrayType      = typeof(string[]);
            var keyConverter   = ConverterManager.Instance.GetConverter(keyType);
            var valueConverter = ConverterManager.Instance.GetConverter(valueType);
            var arrayConverter = ConverterManager.Instance.GetConverter(arrayType);
            return arrayConverter.ConvertToString(((IDictionary)obj).Cast<DictionaryEntry>().Select(kv => $"{keyConverter.ConvertToString(kv.Key, keyType)}{this.separator}{valueConverter.ConvertToString(kv.Value, valueType)}").ToArray(), arrayType);
        }
    }
}