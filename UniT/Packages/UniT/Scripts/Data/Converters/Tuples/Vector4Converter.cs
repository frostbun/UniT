namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public class Vector4Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Vector4);

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = (ITuple)ConverterManager.Instance.ConvertFromString(str, typeof(ValueTuple<float, float, float, float>));
            return new Vector4((float)tuple[0], (float)tuple[1], (float)tuple[2], (float)tuple[3]);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector4)obj;
            return ConverterManager.Instance.ConvertToString((vector.x, vector.y, vector.z, vector.w), typeof(ValueTuple<float, float, float, float>));
        }
    }
}