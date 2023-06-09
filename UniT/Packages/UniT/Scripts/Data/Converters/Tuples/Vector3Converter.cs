namespace UniT.Data.Converters.Tuples
{
    using System;
    using System.Runtime.CompilerServices;
    using UniT.Data.Converters.Base;
    using UnityEngine;

    /// <summary>
    ///     Depends on <see cref="TupleConverter"/>
    /// </summary>
    public class Vector3Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Vector3);

        protected override object ConvertFromString(string str, Type type)
        {
            var tuple = (ITuple)ConverterManager.Instance.ConvertFromString(str, typeof(ValueTuple<float, float, float>));
            return new Vector3((float)tuple[0], (float)tuple[1], (float)tuple[2]);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var vector = (Vector3)obj;
            return ConverterManager.Instance.ConvertToString((vector.x, vector.y, vector.z), typeof(ValueTuple<float, float, float>));
        }
    }
}