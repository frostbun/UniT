#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using Zenject;

    public static class ZenjectBinder
    {
        public static void BindConverterManager(this DiContainer container, IEnumerable<Type>? converterTypes = null)
        {
            #region Converters

            #if UNIT_JSON
            container.BindInterfacesTo<JsonConverter>().AsSingle();
            #endif

            #region Numbers

            container.BindInterfacesTo<Int16Converter>().AsSingle();
            container.BindInterfacesTo<Int32Converter>().AsSingle();
            container.BindInterfacesTo<Int64Converter>().AsSingle();
            container.BindInterfacesTo<UInt16Converter>().AsSingle();
            container.BindInterfacesTo<UInt32Converter>().AsSingle();
            container.BindInterfacesTo<UInt64Converter>().AsSingle();
            container.BindInterfacesTo<SingleConverter>().AsSingle();
            container.BindInterfacesTo<DoubleConverter>().AsSingle();
            container.BindInterfacesTo<DecimalConverter>().AsSingle();

            #endregion

            #region DateTime

            container.BindInterfacesTo<DateTimeConverter>().AsSingle();
            container.BindInterfacesTo<DateTimeOffsetConverter>().AsSingle();

            #endregion

            #region Others

            container.BindInterfacesTo<BooleanConverter>().AsSingle();
            container.BindInterfacesTo<CharConverter>().AsSingle();
            container.BindInterfacesTo<StringConverter>().AsSingle();
            container.BindInterfacesTo<EnumConverter>().AsSingle();
            container.BindInterfacesTo<NullableConverter>().AsSingle();
            container.BindInterfacesTo<GuidConverter>().AsSingle();
            container.BindInterfacesTo<UriConverter>().AsSingle();

            #endregion

            #region Tuples

            container.BindInterfacesTo<TupleConverter>().AsSingle();
            container.BindInterfacesTo<Vector2Converter>().AsSingle();    // Depend on TupleConverter
            container.BindInterfacesTo<Vector3Converter>().AsSingle();    // Depend on TupleConverter
            container.BindInterfacesTo<Vector4Converter>().AsSingle();    // Depend on TupleConverter
            container.BindInterfacesTo<Vector2IntConverter>().AsSingle(); // Depend on TupleConverter
            container.BindInterfacesTo<Vector3IntConverter>().AsSingle(); // Depend on TupleConverter

            #endregion

            #region Collections

            container.BindInterfacesTo<ArrayConverter>().AsSingle();
            container.BindInterfacesTo<ListAndReadOnlyCollectionConverter>().AsSingle(); // Depend on ArrayConverter
            container.BindInterfacesTo<GenericCollectionConverter>().AsSingle();         // Depend on ArrayConverter
            container.BindInterfacesTo<DictionaryConverter>().AsSingle();                // Depend on ArrayConverter
            container.BindInterfacesTo<ReadOnlyDictionaryConverter>().AsSingle();        // Depend on DictionaryConverter
            container.BindInterfacesTo<GenericDictionaryConverter>().AsSingle();         // Depend on DictionaryConverter

            #endregion

            converterTypes?.ForEach(type =>
            {
                if (!typeof(IConverter).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IConverter)}");
                container.BindInterfacesTo(type).AsSingle();
            });

            #endregion

            container.BindInterfacesTo<ConverterManager>().AsSingle();
        }
    }
}
#endif