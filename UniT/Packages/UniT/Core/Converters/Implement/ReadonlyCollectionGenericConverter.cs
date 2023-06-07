namespace UniT.Core.Converters.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class ReadonlyCollectionGenericConverter : ListGenericConverter
    {
        public ReadonlyCollectionGenericConverter(string separator = ";") : base(separator)
        {
        }
        
        protected override Type ConvertibleType => typeof(ReadOnlyCollection<>);
        
        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return Activator.CreateInstance(type, base.ConvertFromString_Internal(str, typeof(List<>).MakeGenericType(type.GetGenericArguments()[0])));
        }
    }
}