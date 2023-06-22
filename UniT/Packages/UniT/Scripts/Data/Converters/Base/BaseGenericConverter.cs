namespace UniT.Data.Converters.Base
{
    using System;
    using UniT.Extensions;

    public abstract class BaseGenericConverter : BaseConverter
    {
        protected override bool CanConvert(Type type)
        {
            return type.DeriveFromGenericType(this.ConvertibleType);
        }
    }
}