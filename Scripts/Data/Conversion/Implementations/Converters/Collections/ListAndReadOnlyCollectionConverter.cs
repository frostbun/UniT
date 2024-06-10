#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    /// <summary>
    ///     Depends on <see cref="ArrayConverter"/>
    /// </summary>
    public sealed class ListAndReadOnlyCollectionConverter : Converter
    {
        [Preserve]
        public ListAndReadOnlyCollectionConverter()
        {
        }

        protected override bool CanConvert(Type type) => type.IsGenericTypeOf(typeof(List<>)) || type.IsGenericTypeOf(typeof(ReadOnlyCollection<>));

        protected override object ConvertFromString(string str, Type type)
        {
            return Activator.CreateInstance(type, this.Manager.ConvertFromString(str, MakeArrayType(type)));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return this.Manager.ConvertToString(((IEnumerable)obj).Cast<object>().ToArray(), MakeArrayType(type));
        }

        private static Type MakeArrayType(Type type)
        {
            return type.GetGenericArguments()[0].MakeArrayType();
        }
    }
}