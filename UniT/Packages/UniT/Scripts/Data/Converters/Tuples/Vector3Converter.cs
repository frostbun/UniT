namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Runtime.CompilerServices;
    using UniT.Data.Converters.Base;
    using UnityEngine;

    public class Vector3Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Vector3);

        protected override object ConvertFromString(string str, Type type)
        {
            var tupleType      = typeof(ValueTuple<float, float, float>);
            var tupleConverter = ConverterManager.Instance.GetConverter(tupleType);
            var tuple          = (ITuple)tupleConverter.ConvertFromString(str, tupleType);
            return new Vector3((float)tuple[0], (float)tuple[1], (float)tuple[2]);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var tupleType      = typeof(ValueTuple<float, float, float>);
            var tupleConverter = ConverterManager.Instance.GetConverter(tupleType);
            var vector         = (Vector3)obj;
            return tupleConverter.ConvertToString((vector.x, vector.y, vector.z), tupleType);
        }
    }
}