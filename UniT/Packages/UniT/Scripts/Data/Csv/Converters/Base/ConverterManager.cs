namespace UniT.Data.Csv.Converters.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Data.Csv.Converters.Implements;

    public class ConverterManager
    {
        public static readonly ConverterManager Instance = new();

        private readonly List<IConverter> converters = new();

        private ConverterManager()
        {
            this.AddConverter(new DefaultConverter());
            this.AddConverter(new Int32Converter());
            this.AddConverter(new SingleConverter());
            this.AddConverter(new DoubleConverter());
            this.AddConverter(new BooleanConverter());
            this.AddConverter(new StringConverter());
            this.AddConverter(new UnityVector2Converter());
            this.AddConverter(new UnityVector3Converter());
            this.AddConverter(new ListGenericConverter());
            this.AddConverter(new ReadonlyCollectionGenericConverter());
            this.AddConverter(new DictionaryGenericConverter());
        }

        public IConverter GetConverter(Type type)
        {
            return this.converters.LastOrDefault(converter => converter.CanConvert(type))
                   ?? throw new($"No converter found for type {type}");
        }

        public void AddConverter(IConverter converter)
        {
            this.converters.Add(converter);
        }
    }
}