namespace UniT.Data
{
    using System;

    public abstract class Converter : IConverter
    {
        bool IConverter.CanConvert(Type type) => this.CanConvert(type);

        object IConverter.ConvertFromString(string str, Type type)
        {
            try
            {
                return this.ConvertFromString(str, type);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot convert '{str}' to '{type.Name}' with '{this.GetType().Name}'", e);
            }
        }

        string IConverter.ConvertToString(object obj, Type type)
        {
            try
            {
                return this.ConvertToString(obj, type);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot convert '{type.Name}' '{obj}' to string with '{this.GetType().Name}'", e);
            }
        }

        protected abstract bool CanConvert(Type type);

        protected abstract object ConvertFromString(string str, Type type);

        protected abstract string ConvertToString(object obj, Type type);
    }

    public abstract class Converter<T> : Converter
    {
        protected sealed override bool CanConvert(Type type) => typeof(T).IsAssignableFrom(type);
    }
}