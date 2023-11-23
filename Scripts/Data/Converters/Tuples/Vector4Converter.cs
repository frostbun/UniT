namespace UniT.Data.Converters
{
    using System;
    using UnityEngine;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public sealed class Vector4Converter : BaseConverter
    {
        private static readonly Type TupleType = typeof((float, float, float, float));

        protected override Type ConvertibleType { get; } = typeof(Vector4);

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = ((float, float, float, float))ConverterManager.Instance.ConvertFromString(str, TupleType);
            return new Vector4(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector4)obj;
            return ConverterManager.Instance.ConvertToString((vector.x, vector.y, vector.z, vector.w), TupleType);
        }
    }
}