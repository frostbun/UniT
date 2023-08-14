namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public class Vector2Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Vector2);

        private static readonly Type TupleType = typeof(ValueTuple<float, float>);

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = (ITuple)ConverterManager.Instance.ConvertFromString(str, TupleType);
            return new Vector2((float)tuple[0], (float)tuple[1]);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector2)obj;
            return ConverterManager.Instance.ConvertToString((vector.x, vector.y), TupleType);
        }
    }
}