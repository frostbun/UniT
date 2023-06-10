namespace UniT.Data.Csv.Converters.Implements
{
    using System;
    using System.Linq;
    using UniT.Data.Csv.Converters.Base;
    using UniT.Extensions;
    using UnityEngine;

    public class UnityVector2Converter : BaseConverter
    {
        private readonly string separator;

        public UnityVector2Converter(string separator = "|")
        {
            this.separator = separator;
        }

        protected override Type ConvertibleType => typeof(Vector2);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            var values = str.Split(this.separator).Select(value => float.Parse(value.IsNullOrWhitespace() ? "0" : value)).ToArray();
            return new Vector2(values[0], values[1]);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            var vector2 = (Vector2)obj;
            return $"{vector2.x}{this.separator}{vector2.y}";
        }
    }
}