namespace UniT.Data.Converters
{
    using System;

    public abstract class BaseConverter : IConverter
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

        protected virtual bool CanConvert(Type type) => this.ConvertibleType.IsAssignableFrom(type);

        protected abstract Type ConvertibleType { get; }

        protected abstract object ConvertFromString(string str, Type type);

        protected abstract string ConvertToString(object obj, Type type);
    }
}