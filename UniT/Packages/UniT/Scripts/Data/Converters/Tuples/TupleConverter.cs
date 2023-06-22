namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UniT.Data.Converters.Base;

    public class TupleConverter : BaseConverter
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
            if (items.Length != itemTypes.Length) throw new ArgumentException($"TupleConverter: Invalid number of items in string. Expected {itemTypes.Length}, got {items.Length}");
            if (items.Length == 0) return ValueTuple.Create();
            var item1 = ConverterManager.Instance.GetConverter(itemTypes[0]).ConvertFromString(items[0], itemTypes[0]);
            if (items.Length == 1) return ValueTuple.Create(item1);
            var item2 = ConverterManager.Instance.GetConverter(itemTypes[1]).ConvertFromString(items[1], itemTypes[1]);
            if (items.Length == 2) return ValueTuple.Create(item1, item2);
            var item3 = ConverterManager.Instance.GetConverter(itemTypes[2]).ConvertFromString(items[2], itemTypes[2]);
            if (items.Length == 3) return ValueTuple.Create(item1, item2, item3);
            var item4 = ConverterManager.Instance.GetConverter(itemTypes[3]).ConvertFromString(items[3], itemTypes[3]);
            if (items.Length == 4) return ValueTuple.Create(item1, item2, item3, item4);
            var item5 = ConverterManager.Instance.GetConverter(itemTypes[4]).ConvertFromString(items[4], itemTypes[4]);
            if (items.Length == 5) return ValueTuple.Create(item1, item2, item3, item4, item5);
            var item6 = ConverterManager.Instance.GetConverter(itemTypes[5]).ConvertFromString(items[5], itemTypes[5]);
            if (items.Length == 6) return ValueTuple.Create(item1, item2, item3, item4, item5, item6);
            var item7 = ConverterManager.Instance.GetConverter(itemTypes[6]).ConvertFromString(items[6], itemTypes[6]);
            if (items.Length == 7) return ValueTuple.Create(item1, item2, item3, item4, item5, item6, item7);
            var item8 = ConverterManager.Instance.GetConverter(itemTypes[7]).ConvertFromString(items[7], itemTypes[7]);
            if (items.Length == 8) return ValueTuple.Create(item1, item2, item3, item4, item5, item6, item7, item8);
            throw new NotSupportedException("Tuples with more than 8 items are not supported");
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var tuple     = (ITuple)obj;
            var itemTypes = type.GetGenericArguments();
            var str       = new StringBuilder();
            for (var i = 0; i < itemTypes.Length; ++i)
            {
                if (i > 0) str.Append(this.separator);
                var item          = tuple[i];
                var itemType      = itemTypes[i];
                var itemConverter = ConverterManager.Instance.GetConverter(itemType);
                str.Append(itemConverter.ConvertToString(item, itemType));
            }

            return str.ToString();
        }
    }
}