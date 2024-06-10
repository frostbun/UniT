#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    public sealed class ArrayConverter : Converter<Array>
    {
        private readonly string separator;

        [Preserve]
        public ArrayConverter(string separator = ";")
        {
            this.separator = separator;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            var elementType      = type.GetElementType()!;
            var elementConverter = this.Manager.GetConverter(elementType);
            var elements         = str.Split(this.separator);
            var array            = Array.CreateInstance(elementType, elements.Length);
            for (var i = 0; i < elements.Length; ++i)
            {
                array.SetValue(elementConverter.ConvertFromString(elements[i], elementType), i);
            }
            return array;
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var elementType      = type.GetElementType()!;
            var elementConverter = this.Manager.GetConverter(elementType);
            return ((Array)obj).Cast<object>()
                .Select(element => elementConverter.ConvertToString(element, elementType))
                .Join(this.separator);
        }
    }
}