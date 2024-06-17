#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using VContainer;

    public static class VContainerBinder
    {
        public static void RegisterConverterManager(this IContainerBuilder builder, IEnumerable<Type>? converterTypes = null)
        {
            #region Converters

            #if UNIT_JSON
            builder.Register<JsonConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif

            #region Numbers

            builder.Register<Int16Converter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<Int32Converter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<Int64Converter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<UInt16Converter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<UInt32Converter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<UInt64Converter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SingleConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DoubleConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DecimalConverter>(Lifetime.Singleton).AsImplementedInterfaces();

            #endregion

            #region DateTime

            builder.Register<DateTimeConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DateTimeOffsetConverter>(Lifetime.Singleton).AsImplementedInterfaces();

            #endregion

            #region Others

            builder.Register<BooleanConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CharConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<StringConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EnumConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<NullableConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GuidConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<UriConverter>(Lifetime.Singleton).AsImplementedInterfaces();

            #endregion

            #region Tuples

            builder.Register<TupleConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<Vector2Converter>(Lifetime.Singleton).AsImplementedInterfaces();    // Depend on TupleConverter
            builder.Register<Vector3Converter>(Lifetime.Singleton).AsImplementedInterfaces();    // Depend on TupleConverter
            builder.Register<Vector4Converter>(Lifetime.Singleton).AsImplementedInterfaces();    // Depend on TupleConverter
            builder.Register<Vector2IntConverter>(Lifetime.Singleton).AsImplementedInterfaces(); // Depend on TupleConverter
            builder.Register<Vector3IntConverter>(Lifetime.Singleton).AsImplementedInterfaces(); // Depend on TupleConverter

            #endregion

            #region Collections

            builder.Register<ArrayConverter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ListAndReadOnlyCollectionConverter>(Lifetime.Singleton).AsImplementedInterfaces(); // Depend on ArrayConverter
            builder.Register<GenericCollectionConverter>(Lifetime.Singleton).AsImplementedInterfaces();         // Depend on ArrayConverter
            builder.Register<DictionaryConverter>(Lifetime.Singleton).AsImplementedInterfaces();                // Depend on ArrayConverter
            builder.Register<ReadOnlyDictionaryConverter>(Lifetime.Singleton).AsImplementedInterfaces();        // Depend on DictionaryConverter
            builder.Register<GenericDictionaryConverter>(Lifetime.Singleton).AsImplementedInterfaces();         // Depend on DictionaryConverter

            #endregion

            converterTypes?.ForEach(type =>
            {
                if (!typeof(IConverter).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IConverter)}");
                builder.Register(type, Lifetime.Singleton).AsImplementedInterfaces();
            });

            #endregion

            builder.Register<ConverterManager>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif