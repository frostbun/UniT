namespace UniT.Data.Converters.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Data.Converters.Collections;
    using UniT.Data.Converters.DateTime;
    using UniT.Data.Converters.Others;
    using UniT.Data.Converters.Primitives;
    using UniT.Data.Converters.Tuples;

    public class ConverterManager
    {
        private static ConverterManager _instance;
        public static  ConverterManager Instance  => _instance ??= new();
        public static  void             Dispose() => _instance = null;

        private readonly List<IConverter> converters = new();

        private ConverterManager()
        {
            this.AddConverter(new DefaultConverter());

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
            this.AddConverter(new ListGenericConverter()); // Depend on ArrayConverter
            this.AddConverter(new DictionaryGenericConverter()); // Depend on ArrayConverter
            this.AddConverter(new ReadonlyCollectionGenericConverter()); // Depend on ListGenericConverter
            this.AddConverter(new ReadonlyDictionaryGenericConverter()); // Depend on DictionaryGenericConverter

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
                   ?? throw new($"No converter found for type {type.Name}");
        }

        public void AddConverter(IConverter converter)
        {
            this.converters.Add(converter);
        }
    }
}