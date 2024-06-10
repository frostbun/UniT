#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    /// <summary>
    ///     Depends on <see cref="ArrayConverter"/>
    /// </summary>
    public sealed class DictionaryConverter : Converter
    {
        private readonly string separator;

        [Preserve]
        public DictionaryConverter(string separator = ":")
        {
            this.separator = separator;
        }

        protected override bool CanConvert(Type type) => type.IsGenericTypeOf(typeof(Dictionary<,>));

        private static readonly Type ArrayType = typeof(string[]);

        protected override object ConvertFromString(string str, Type type)
        {
            var keyType        = type.GetGenericArguments()[0];
            var valueType      = type.GetGenericArguments()[1];
            var keyConverter   = this.Manager.GetConverter(keyType);
            var valueConverter = this.Manager.GetConverter(valueType);
            var dictionary     = (IDictionary)Activator.CreateInstance(type);
            foreach (var item in (string[])this.Manager.ConvertFromString(str, ArrayType))
            {
                var kv = item.Split(this.separator);
                dictionary.Add(
                    keyConverter.ConvertFromString(kv[0], keyType),
                    valueConverter.ConvertFromString(kv[1], valueType)
                );
            }
            return dictionary;
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var keyType        = type.GetGenericArguments()[0];
            var valueType      = type.GetGenericArguments()[1];
            var keyConverter   = this.Manager.GetConverter(keyType);
            var valueConverter = this.Manager.GetConverter(valueType);
            return this.Manager.ConvertToString(
                ((IDictionary)obj).Cast<DictionaryEntry>()
                .Select(kv => string.Concat(
                    keyConverter.ConvertToString(kv.Key, keyType),
                    this.separator,
                    valueConverter.ConvertToString(kv.Value, valueType)
                ))
                .ToArray(),
                ArrayType
            );
        }
    }
}