namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Runtime.CompilerServices;
    using UniT.Data.Converters.Base;
    using UnityEngine;

    public class Vector2Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Vector2);

        protected override object ConvertFromString(string str, Type type)
        {
            var tupleType      = typeof(ValueTuple<float, float>);
            var tupleConverter = ConverterManager.Instance.GetConverter(tupleType);
            var tuple          = (ITuple)tupleConverter.ConvertFromString(str, tupleType);
            return new Vector2((float)tuple[0], (float)tuple[1]);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var tupleType      = typeof(ValueTuple<float, float>);
            var tupleConverter = ConverterManager.Instance.GetConverter(tupleType);
            var vector         = (Vector2)obj;
            return tupleConverter.ConvertToString((vector.x, vector.y), tupleType);
        }
    }
}