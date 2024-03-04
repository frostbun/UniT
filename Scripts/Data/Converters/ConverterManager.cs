namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public sealed class ConverterManager
    {
        public static ConverterManager Instance { get; }

        static ConverterManager()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            Instance                                = new ConverterManager();
        }

        private readonly List<IConverter> converters = new List<IConverter>();

        private ConverterManager()
        {
            #if UNIT_NEWTONSOFT_JSON
            this.AddConverter(new JsonConverter()); // Default converter
            #endif

            #region Primitives

            this.AddConverter(new Int16Converter());
            this.AddConverter(new Int32Converter());
            this.AddConverter(new Int64Converter());
            this.AddConverter(new UInt16Converter());
            this.AddConverter(new UInt32Converter());
            this.AddConverter(new UInt64Converter());
            this.AddConverter(new SingleConverter());
            this.AddConverter(new DoubleConverter());
            this.AddConverter(new DecimalConverter());
            this.AddConverter(new BooleanConverter());
            this.AddConverter(new CharConverter());
            this.AddConverter(new StringConverter());
            this.AddConverter(new EnumConverter());

            #endregion

            #region Tuples

            this.AddConverter(new TupleConverter());
            this.AddConverter(new Vector2Converter()); // Depend on TupleConverter
            this.AddConverter(new Vector3Converter()); // Depend on TupleConverter
            this.AddConverter(new Vector4Converter()); // Depend on TupleConverter

            #endregion

            #region Collections

            this.AddConverter(new ArrayConverter());
            this.AddConverter(new ListConverter());               // Depend on ArrayConverter
            this.AddConverter(new DictionaryConverter());         // Depend on ArrayConverter
            this.AddConverter(new ReadOnlyCollectionConverter()); // Depend on ArrayConverter
            this.AddConverter(new ReadOnlyDictionaryConverter()); // Depend on DictionaryConverter

            #endregion

            #region DateTime

            this.AddConverter(new DateTimeConverter());
            this.AddConverter(new DateTimeOffsetConverter());

            #endregion

            #region Others

            this.AddConverter(new UriConverter());
            this.AddConverter(new GuidConverter());

            #endregion
        }

        public IConverter GetConverter(Type type)
        {
            return this.converters.LastOrDefault(converter => converter.CanConvert(type))
                ?? throw new ArgumentOutOfRangeException(nameof(type), type, $"No converter found for type {type.Name}");
        }

        public void AddConverter(IConverter converter)
        {
            this.converters.Add(converter);
        }

        public object ConvertFromString(string str, Type type)
        {
            return this.GetConverter(type).ConvertFromString(str, type);
        }

        public string ConvertToString(object obj, Type type)
        {
            return this.GetConverter(type).ConvertToString(obj, type);
        }
    }
}