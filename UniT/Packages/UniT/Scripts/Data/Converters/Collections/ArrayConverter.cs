namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Linq;
    using UniT.Data.Converters.Base;

    public class ArrayConverter : BaseConverter
    {
        private readonly string separator;

        public ArrayConverter(string separator = ";")
        {
            this.separator = separator;
        }

        protected override Type ConvertibleType => typeof(Array);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            var elementType      = type.GetElementType();
            var elementConverter = ConverterManager.Instance.GetConverter(elementType);
            var elements         = str.Split(this.separator);
            var array            = Array.CreateInstance(elementType, elements.Length);
            for (var i = 0; i < elements.Length; ++i)
            {
                array.SetValue(elementConverter.ConvertFromString(elements[i], elementType), i);
            }

            return array;
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            var elementType      = type.GetElementType();
            var elementConverter = ConverterManager.Instance.GetConverter(elementType);
            return string.Join(this.separator, ((Array)obj).Cast<object>().Select(element => elementConverter.ConvertToString(element, elementType)));
        }
    }
}