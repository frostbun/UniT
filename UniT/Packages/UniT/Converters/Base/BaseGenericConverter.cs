namespace UniT.Converters.Base
{
    using System;

    public abstract class BaseGenericConverter : BaseConverter
    {
        public override bool CanConvert(Type type)
        {
            return type.IsGenericType && this.ConvertibleType.IsAssignableFrom(type.GetGenericTypeDefinition());
        }
    }
}