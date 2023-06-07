namespace UniT.Core.Converters.Base
{
    using System;

    public abstract class BaseConverter : IConverter
    {
        protected abstract Type   ConvertibleType { get; }
        protected abstract object ConvertFromString_Internal(string str, Type type);
        protected abstract string ConvertToString_Internal(object obj, Type type);

        public virtual bool CanConvert(Type type)
        {
            return this.ConvertibleType.IsAssignableFrom(type);
        }

        public object ConvertFromString(string str, Type type)
        {
            try
            {
                return this.ConvertFromString_Internal(str, type);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot convert '{str}' to '{type.Name}' with '{this.GetType().Name}'", e);
            }
        }

        public string ConvertToString(object obj, Type type)
        {
            try
            {
                return this.ConvertToString_Internal(obj, type);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot convert '{type.Name}' '{obj}' to string with '{this.GetType().Name}'", e);
            }
        }
    }
}