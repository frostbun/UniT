#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public sealed class Vector2Converter : Converter<Vector2>
    {
        [Preserve]
        public Vector2Converter()
        {
        }

        private static readonly Type TupleType = typeof((float, float));

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = ((float, float))this.Manager.ConvertFromString(str, TupleType);
            return new Vector2(tuple.Item1, tuple.Item2);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector2)obj;
            return this.Manager.ConvertToString((vector.x, vector.y), TupleType);
        }
    }
}