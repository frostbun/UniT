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
    public sealed class GenericCollectionConverter : Converter
    {
        [Preserve]
        public GenericCollectionConverter()
        {
        }

        protected override bool CanConvert(Type type) =>
            type.IsGenericTypeOf(typeof(ICollection<>))
            || type.IsGenericTypeOf(typeof(IReadOnlyCollection<>))
            || type.IsGenericTypeOf(typeof(IList<>))
            || type.IsGenericTypeOf(typeof(IReadOnlyList<>));

        protected override object ConvertFromString(string str, Type type)
        {
            return this.Manager.ConvertFromString(str, MakeArrayType(type));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return this.Manager.ConvertToString(obj as Array ?? ((IEnumerable)obj).Cast<object>().ToArray(), MakeArrayType(type));
        }

        private static Type MakeArrayType(Type type)
        {
            return type.GetGenericArguments()[0].MakeArrayType();
        }
    }
}