#if UNIT_DI
#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Collections.Generic;
    using UniT.DI;
    using UniT.Extensions;

    public static class DIBinder
    {
        public static void AddConverterManager(this DependencyContainer container, IEnumerable<Type>? converterTypes = null)
        {
            #region Converters

            #if UNIT_JSON
            container.AddInterfaces<JsonConverter>();
            #endif

            #region Numbers

            container.AddInterfaces<Int16Converter>();
            container.AddInterfaces<Int32Converter>();
            container.AddInterfaces<Int64Converter>();
            container.AddInterfaces<UInt16Converter>();
            container.AddInterfaces<UInt32Converter>();
            container.AddInterfaces<UInt64Converter>();
            container.AddInterfaces<SingleConverter>();
            container.AddInterfaces<DoubleConverter>();
            container.AddInterfaces<DecimalConverter>();

            #endregion

            #region DateTime

            container.AddInterfaces<DateTimeConverter>();
            container.AddInterfaces<DateTimeOffsetConverter>();

            #endregion

            #region Others

            container.AddInterfaces<BooleanConverter>();
            container.AddInterfaces<CharConverter>();
            container.AddInterfaces<StringConverter>();
            container.AddInterfaces<EnumConverter>();
            container.AddInterfaces<NullableConverter>();
            container.AddInterfaces<GuidConverter>();
            container.AddInterfaces<UriConverter>();

            #endregion

            #region Tuples

            container.AddInterfaces<TupleConverter>();
            container.AddInterfaces<Vector2Converter>();    // Depend on TupleConverter
            container.AddInterfaces<Vector3Converter>();    // Depend on TupleConverter
            container.AddInterfaces<Vector4Converter>();    // Depend on TupleConverter
            container.AddInterfaces<Vector2IntConverter>(); // Depend on TupleConverter
            container.AddInterfaces<Vector3IntConverter>(); // Depend on TupleConverter

            #endregion

            #region Collections

            container.AddInterfaces<ArrayConverter>();
            container.AddInterfaces<ListAndReadOnlyCollectionConverter>(); // Depend on ArrayConverter
            container.AddInterfaces<GenericCollectionConverter>();         // Depend on ArrayConverter
            container.AddInterfaces<DictionaryConverter>();                // Depend on ArrayConverter
            container.AddInterfaces<ReadOnlyDictionaryConverter>();        // Depend on DictionaryConverter
            container.AddInterfaces<GenericDictionaryConverter>();         // Depend on DictionaryConverter

            #endregion

            converterTypes?.ForEach(type =>
            {
                if (!typeof(IConverter).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IConverter)}");
                container.AddInterfaces(type);
            });

            #endregion

            container.AddInterfaces<ConverterManager>();
        }
    }
}
#endif