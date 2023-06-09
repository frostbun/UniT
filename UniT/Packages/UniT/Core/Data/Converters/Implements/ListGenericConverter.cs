namespace UniT.Core.Data.Converters.Implements
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Core.Data.Converters.Base;

    public class ListGenericConverter : BaseGenericConverter
    {
        private readonly string separator;

        public ListGenericConverter(string separator = ";")
        {
            this.separator = separator;
        }

        protected override Type ConvertibleType => typeof(List<>);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            var itemType      = type.GetGenericArguments()[0];
            var itemConverter = ConverterManager.Instance.GetConverter(itemType);
            var list          = (IList)Activator.CreateInstance(type);
            foreach (var item in str.Split(this.separator))
            {
                list.Add(itemConverter.ConvertFromString(item, itemType));
            }

            return list;
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            var itemType      = type.GetGenericArguments()[0];
            var itemConverter = ConverterManager.Instance.GetConverter(itemType);
            return string.Join(this.separator, ((IList)obj).Cast<object>().Select(item => itemConverter.ConvertToString(item, itemType)));
        }
    }
}