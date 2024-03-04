namespace UniT.Data
{
    using System;

    public interface IConverter
    {
        public bool CanConvert(Type type);

        public object ConvertFromString(string str, Type type);

        public string ConvertToString(object obj, Type type);
    }
}