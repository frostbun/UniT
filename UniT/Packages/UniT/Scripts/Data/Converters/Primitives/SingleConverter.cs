namespace UniT.Data.Converters.Primitives
{
    using System;
    using UniT.Data.Converters.Base;

    public class SingleConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(float);

        protected override object ConvertFromString(string str, Type type)
        {
            return float.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}