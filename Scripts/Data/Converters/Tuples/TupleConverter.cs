namespace UniT.Data.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UniT.Extensions;

    public sealed class TupleConverter : BaseConverter
    {
        private readonly string separator;

        public TupleConverter(string separator = "|")
        {
            this.separator = separator;
        }

        protected override Type ConvertibleType => typeof(ITuple);

        protected override object ConvertFromString(string str, Type type)
        {
            var items     = str.Split(this.separator);
            var itemTypes = type.GetGenericArguments();
            return Activator.CreateInstance(type, IterTools.StrictZip(items, itemTypes, ConverterManager.Instance.ConvertFromString).ToArray());
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var tuple     = (ITuple)obj;
            var itemTypes = type.GetGenericArguments();
            return string.Join(this.separator, IterTools.StrictZip(ToEnumerable(tuple), itemTypes, ConverterManager.Instance.ConvertToString));
        }

        private static IEnumerable<object> ToEnumerable(ITuple tuple)
        {
            for (var i = 0; i < tuple.Length; ++i) yield return tuple[i];
        }
    }
}