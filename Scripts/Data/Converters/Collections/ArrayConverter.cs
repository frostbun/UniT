namespace UniT.Data
{
    using System;
    using System.Linq;

    public sealed class ArrayConverter : Converter<Array>
    {
        private readonly string separator;

        public ArrayConverter(string separator = ";")
        {
            this.separator = separator;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            var elementType      = type.GetElementType()!;
            var elementConverter = ConverterManager.GetConverter(elementType);
            var elements         = str.Split(new[] { this.separator }, StringSplitOptions.None);
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
            var elementConverter = ConverterManager.GetConverter(elementType);
            return string.Join(this.separator, ((Array)obj).Cast<object>().Select(element => elementConverter.ConvertToString(element, elementType)));
        }
    }
}