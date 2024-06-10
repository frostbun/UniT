#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public sealed class Vector3IntConverter : Converter<Vector3Int>
    {
        [Preserve]
        public Vector3IntConverter()
        {
        }

        private static readonly Type TupleType = typeof((int, int, int));

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = ((int, int, int))this.Manager.ConvertFromString(str, TupleType);
            return new Vector3Int(tuple.Item1, tuple.Item2, tuple.Item3);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector3Int)obj;
            return this.Manager.ConvertToString((vector.x, vector.y, vector.z), TupleType);
        }
    }
}