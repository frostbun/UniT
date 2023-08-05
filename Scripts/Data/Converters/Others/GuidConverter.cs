namespace UniT.Data.Converters.Others
{
    using System;

    public class GuidConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Guid);

        protected override object ConvertFromString(string str, Type type)
        {
            return new Guid(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}