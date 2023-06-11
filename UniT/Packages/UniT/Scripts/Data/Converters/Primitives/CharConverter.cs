namespace UniT.Data.Converters.Primitives
{
    using System;
    using UniT.Data.Converters.Base;

    public class CharConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(char);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return char.Parse(str);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}