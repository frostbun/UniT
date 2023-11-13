namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Depends on <see cref="ArrayConverter"/>
    /// </summary>
    public sealed class DictionaryConverter : BaseConverter
    {
        private readonly string _separator;

        public DictionaryConverter(string separator = ":")
        {
            this._separator = separator;
        }

        protected override Type ConvertibleType => typeof(Dictionary<,>);

        private static readonly Type ArrayType = typeof(string[]);

        protected override object ConvertFromString(string str, Type type)
        {
            var keyType        = type.GetGenericArguments()[0];
            var valueType      = type.GetGenericArguments()[1];
            var keyConverter   = ConverterManager.Instance.GetConverter(keyType);
            var valueConverter = ConverterManager.Instance.GetConverter(valueType);
            var dictionary     = (IDictionary)Activator.CreateInstance(type);
            foreach (var item in (string[])ConverterManager.Instance.ConvertFromString(str, ArrayType))
            {
                var kv = item.Split(this._separator);
                dictionary.Add(keyConverter.ConvertFromString(kv[0], keyType), valueConverter.ConvertFromString(kv[1], valueType));
            }
            return dictionary;
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var keyType        = type.GetGenericArguments()[0];
            var valueType      = type.GetGenericArguments()[1];
            var keyConverter   = ConverterManager.Instance.GetConverter(keyType);
            var valueConverter = ConverterManager.Instance.GetConverter(valueType);
            return ConverterManager.Instance.ConvertToString(((IDictionary)obj).Cast<DictionaryEntry>().Select(kv => $"{keyConverter.ConvertToString(kv.Key, keyType)}{this._separator}{valueConverter.ConvertToString(kv.Value, valueType)}").ToArray(), ArrayType);
        }
    }
}