namespace UniT.Data.Converters.Tuples
{
    using System;
    using UniT.Data.Converters.Base;
    using UnityEngine;

    public class UnityVector4Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Vector2);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            var tupleType      = typeof(ValueTuple<float, float, float, float>);
            var tupleConverter = ConverterManager.Instance.GetConverter(tupleType);
            var tuple          = (ValueTuple<float, float, float, float>)tupleConverter.ConvertFromString(str, tupleType);
            return new Vector4(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            var tupleType      = typeof(ValueTuple<float, float, float, float>);
            var tupleConverter = ConverterManager.Instance.GetConverter(tupleType);
            var vector         = (Vector4)obj;
            return tupleConverter.ConvertToString((vector.x, vector.y, vector.z, vector.w), tupleType);
        }
    }
}