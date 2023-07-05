namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Runtime.CompilerServices;
    using UniT.Data.Converters.Base;
    using UnityEngine;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public class Vector2Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Vector2);

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = (ITuple)ConverterManager.Instance.ConvertFromString(str, typeof(ValueTuple<float, float>));
            return new Vector2((float)tuple[0], (float)tuple[1]);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector2)obj;
            return ConverterManager.Instance.ConvertToString((vector.x, vector.y), typeof(ValueTuple<float, float>));
        }
    }
}