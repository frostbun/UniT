namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UniT.Extensions;

    public sealed class TupleConverter : BaseConverter
    {
        private readonly string _separator;

        public TupleConverter(string separator = "|")
        {
            this._separator = separator;
        }

        protected override Type ConvertibleType => typeof(ITuple);

        protected override object ConvertFromString(string str, Type type)
        {
            var items     = str.Split(this._separator);
            var itemTypes = type.GetGenericArguments();
            if (items.Length != itemTypes.Length) throw new ArgumentException($"TupleConverter: Invalid number of items in string. Expected {itemTypes.Length}, got {items.Length}.");
            return Activator.CreateInstance(type, IterTools.Zip(items, itemTypes, ConverterManager.Instance.ConvertFromString).ToArray());
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var tuple     = (ITuple)obj;
            var itemTypes = type.GetGenericArguments();
            if (tuple.Length != itemTypes.Length) throw new ArgumentException($"TupleConverter: Invalid number of items in tuple. Expected {itemTypes.Length}, got {tuple.Length}.");
            return string.Join(this._separator, IterTools.Zip(ToEnumerable(tuple), itemTypes, ConverterManager.Instance.ConvertToString));
        }

        private static IEnumerable<object> ToEnumerable(ITuple tuple)
        {
            for (var i = 0; i < tuple.Length; ++i) yield return tuple[i];
        }
    }
}