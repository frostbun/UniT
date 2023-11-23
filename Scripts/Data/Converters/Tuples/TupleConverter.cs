namespace UniT.Data.Converters
{
    using System;
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

        protected override Type ConvertibleType { get; } = typeof(ITuple);

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
            return string.Join(this.separator, IterTools.StrictZip(tuple.ToEnumerable(), itemTypes, ConverterManager.Instance.ConvertToString));
        }
    }
}