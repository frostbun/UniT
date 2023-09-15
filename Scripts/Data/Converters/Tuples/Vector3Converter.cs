namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public sealed class Vector3Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Vector3);

        private static readonly Type TupleType = typeof(ValueTuple<float, float, float>);

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = (ITuple)ConverterManager.Instance.ConvertFromString(str, TupleType);
            return new Vector3((float)tuple[0], (float)tuple[1], (float)tuple[2]);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector3)obj;
            return ConverterManager.Instance.ConvertToString((vector.x, vector.y, vector.z), TupleType);
        }
    }
}