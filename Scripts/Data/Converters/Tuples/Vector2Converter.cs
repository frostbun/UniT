namespace UniT.Data.Converters
{
    using System;
    using UnityEngine;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public sealed class Vector2Converter : BaseConverter
    {
        private static readonly Type TupleType = typeof((float, float));

        protected override Type ConvertibleType { get; } = typeof(Vector2);

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = ((float, float))ConverterManager.Instance.ConvertFromString(str, TupleType);
            return new Vector2(tuple.Item1, tuple.Item2);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector2)obj;
            return ConverterManager.Instance.ConvertToString((vector.x, vector.y), TupleType);
        }
    }
}