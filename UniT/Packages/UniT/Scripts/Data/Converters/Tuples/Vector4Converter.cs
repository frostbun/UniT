namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Runtime.CompilerServices;
    using UniT.Data.Converters.Base;
    using UnityEngine;

    public class Vector4Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Vector4);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            var tupleType      = typeof(ValueTuple<float, float, float, float>);
            var tupleConverter = ConverterManager.Instance.GetConverter(tupleType);
            var tuple          = (ITuple)tupleConverter.ConvertFromString(str, tupleType);
            return new Vector4((float)tuple[0], (float)tuple[1], (float)tuple[2], (float)tuple[3]);
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