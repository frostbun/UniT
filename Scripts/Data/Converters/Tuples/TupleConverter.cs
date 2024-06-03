#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UniT.Extensions;

    public sealed class TupleConverter : Converter<ITuple>
    {
        private readonly string separator;

        public TupleConverter(string separator = "|")
        {
            this.separator = separator;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            var items     = str.Split(this.separator);
            var itemTypes = type.GetGenericArguments();
            return Activator.CreateInstance(type, IterTools.Zip(items, itemTypes, ConverterManager.ConvertFromString).ToArray());
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var tuple     = (ITuple)obj;
            var itemTypes = type.GetGenericArguments();
            return IterTools.Zip(ToEnumerable(tuple), itemTypes, ConverterManager.ConvertToString).Join(this.separator);

            static IEnumerable<object> ToEnumerable(ITuple tuple)
            {
                for (var i = 0; i < tuple.Length; ++i) yield return tuple[i];
            }
        }
    }
}