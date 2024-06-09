#nullable enable
namespace UniT.Data.Conversion
{
    using System;

    public interface IConverterManager
    {
        public IConverter GetConverter(Type type);

        public object ConvertFromString(string str, Type type) => this.GetConverter(type).ConvertFromString(str, type);

        public string ConvertToString(object obj, Type type) => this.GetConverter(type).ConvertToString(obj, type);
    }
}