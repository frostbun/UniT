#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using UniT.Extensions;

    /// <summary>
    ///     Depends on <see cref="ArrayConverter"/>
    /// </summary>
    public sealed class ListAndReadOnlyCollectionConverter : Converter
    {
        protected override bool CanConvert(Type type) => type.DerivesFrom(typeof(List<>)) || type.DerivesFrom(typeof(ReadOnlyCollection<>));

        protected override object ConvertFromString(string str, Type type)
        {
            return Activator.CreateInstance(type, ConverterManager.ConvertFromString(str, MakeArrayType(type)));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ConverterManager.ConvertToString(((IEnumerable)obj).Cast<object>().ToArray(), MakeArrayType(type));
        }

        private static Type MakeArrayType(Type type)
        {
            return type.GetGenericArguments()[0].MakeArrayType();
        }
    }
}