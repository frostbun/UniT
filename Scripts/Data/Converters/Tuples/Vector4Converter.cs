namespace UniT.Data.Converters
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public sealed class Vector4Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Vector4);

        private static readonly Type TupleType = typeof(ValueTuple<float, float, float, float>);

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = (ITuple)ConverterManager.Instance.ConvertFromString(str, TupleType);
            return new Vector4((float)tuple[0], (float)tuple[1], (float)tuple[2], (float)tuple[3]);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector4)obj;
            return ConverterManager.Instance.ConvertToString((vector.x, vector.y, vector.z, vector.w), TupleType);
        }
    }
}