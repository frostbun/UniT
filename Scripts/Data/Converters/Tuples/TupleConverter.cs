namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UniT.Extensions;

    public class TupleConverter : BaseConverter
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
            if (items.Length != itemTypes.Length) throw new ArgumentException($"TupleConverter: Invalid number of items in string. Expected {itemTypes.Length}, got {items.Length}");
            return Activator.CreateInstance(type, IterTools.Zip(items, itemTypes, (item, itemType) => ConverterManager.Instance.ConvertFromString(item, itemType)).ToArray());
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var tuple     = (ITuple)obj;
            var itemTypes = type.GetGenericArguments();
            var str       = new StringBuilder();
            for (var i = 0; i < itemTypes.Length; ++i)
            {
                if (i > 0) str.Append(this._separator);
                str.Append(ConverterManager.Instance.ConvertToString(tuple[i], itemTypes[i]));
            }
            return str.ToString();
        }
    }
}