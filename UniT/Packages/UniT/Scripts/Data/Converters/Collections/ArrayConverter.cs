namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Linq;

    public class ArrayConverter : BaseConverter
    {
        private readonly string _separator;

        public ArrayConverter(string separator = ";")
        {
            this._separator = separator;
        }

        protected override Type ConvertibleType => typeof(Array);

        protected override object ConvertFromString(string str, Type type)
        {
            var elementType      = type.GetElementType();
            var elementConverter = ConverterManager.Instance.GetConverter(elementType);
            var elements         = str.Split(this._separator);
            var array            = Array.CreateInstance(elementType, elements.Length);
            for (var i = 0; i < elements.Length; ++i)
            {
                array.SetValue(elementConverter.ConvertFromString(elements[i], elementType), i);
            }

            return array;
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var elementType      = type.GetElementType();
            var elementConverter = ConverterManager.Instance.GetConverter(elementType);
            return string.Join(this._separator, ((Array)obj).Cast<object>().Select(element => elementConverter.ConvertToString(element, elementType)));
        }
    }
}