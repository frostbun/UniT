#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class ConverterManager
    {
        private static readonly List<IConverter> Converters = new List<IConverter>();

        static ConverterManager()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            #if UNIT_NEWTONSOFT_JSON
            AddConverter(new JsonConverter()); // Default converter
            #endif

            #region Primitives

            AddConverter(new Int16Converter());
            AddConverter(new Int32Converter());
            AddConverter(new Int64Converter());
            AddConverter(new UInt16Converter());
            AddConverter(new UInt32Converter());
            AddConverter(new UInt64Converter());
            AddConverter(new SingleConverter());
            AddConverter(new DoubleConverter());
            AddConverter(new DecimalConverter());
            AddConverter(new BooleanConverter());
            AddConverter(new CharConverter());
            AddConverter(new StringConverter());
            AddConverter(new EnumConverter());

            #endregion

            #region Tuples

            AddConverter(new TupleConverter());
            AddConverter(new Vector2Converter()); // Depend on TupleConverter
            AddConverter(new Vector3Converter()); // Depend on TupleConverter
            AddConverter(new Vector4Converter()); // Depend on TupleConverter

            #endregion

            #region Collections

            AddConverter(new ArrayConverter());
            AddConverter(new ListAndReadOnlyCollectionConverter()); // Depend on ArrayConverter
            AddConverter(new DictionaryConverter());                // Depend on ArrayConverter
            AddConverter(new ReadOnlyDictionaryConverter());        // Depend on DictionaryConverter

            #endregion

            #region DateTime

            AddConverter(new DateTimeConverter());
            AddConverter(new DateTimeOffsetConverter());

            #endregion

            #region Others

            AddConverter(new UriConverter());
            AddConverter(new GuidConverter());
            AddConverter(new NullableConverter());

            #endregion
        }

        public static void AddConverter(IConverter converter)
        {
            Converters.Add(converter);
        }

        public static IConverter GetConverter(Type type)
        {
            return Converters.LastOrDefault(converter => converter.CanConvert(type))
                ?? throw new ArgumentOutOfRangeException(nameof(type), type, $"No converter found for {type.Name}");
        }

        public static object ConvertFromString(string str, Type type)
        {
            return GetConverter(type).ConvertFromString(str, type);
        }

        public static string ConvertToString(object obj, Type type)
        {
            return GetConverter(type).ConvertToString(obj, type);
        }
    }
}