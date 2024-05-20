namespace UniT.Data
{
    using System;
    using UnityEngine;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public sealed class Vector3Converter : Converter<Vector3>
    {
        private static readonly Type TupleType = typeof((float, float, float));

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = ((float, float, float))ConverterManager.ConvertFromString(str, TupleType);
            return new Vector3(tuple.Item1, tuple.Item2, tuple.Item3);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector3)obj;
            return ConverterManager.ConvertToString((vector.x, vector.y, vector.z), TupleType);
        }
    }
}