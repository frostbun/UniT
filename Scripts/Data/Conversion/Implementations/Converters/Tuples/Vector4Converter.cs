#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public sealed class Vector4Converter : Converter<Vector4>
    {
        [Preserve]
        public Vector4Converter()
        {
        }

        private static readonly Type TupleType = typeof((float, float, float, float));

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = ((float, float, float, float))this.Manager.ConvertFromString(str, TupleType);
            return new Vector4(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector4)obj;
            return this.Manager.ConvertToString((vector.x, vector.y, vector.z, vector.w), TupleType);
        }
    }
}