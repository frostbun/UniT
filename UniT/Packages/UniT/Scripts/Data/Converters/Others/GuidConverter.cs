namespace UniT.Data.Converters.Others
{
    using System;
    using UniT.Data.Converters.Base;

    public class GuidConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Guid);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return new Guid(str);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}