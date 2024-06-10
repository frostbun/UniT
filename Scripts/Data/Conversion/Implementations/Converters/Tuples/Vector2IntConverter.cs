#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public sealed class Vector2IntConverter : Converter<Vector2Int>
    {
        [Preserve]
        public Vector2IntConverter()
        {
        }

        private static readonly Type TupleType = typeof((int, int));

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = ((int, int))this.Manager.ConvertFromString(str, TupleType);
            return new Vector2Int(tuple.Item1, tuple.Item2);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector2Int)obj;
            return this.Manager.ConvertToString((vector.x, vector.y), TupleType);
        }
    }
}