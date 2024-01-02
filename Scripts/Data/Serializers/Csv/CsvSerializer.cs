namespace UniT.Data.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Sylvan.Data.Csv;
    using UniT.Data.Converters;
    using UniT.Data.Types;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    public sealed class CsvSerializer : ISerializer
    {
        [Preserve]
        public CsvSerializer()
        {
        }

        bool ISerializer.CanSerialize(Type type) => typeof(ICsvData).IsAssignableFrom(type);

        void ISerializer.Populate(IData data, string rawData)
        {
            using var reader = CsvDataReader.Create(new StringReader(rawData), new CsvDataReaderOptions { HeaderComparer = StringComparer.OrdinalIgnoreCase, });
            var       parser = new CsvParser((ICsvData)data, reader);
            while (reader.Read()) parser.Parse();
        }

        string ISerializer.Serialize(IData data)
        {
            throw new NotImplementedException();
        }

        private sealed class CsvParser
        {
            private readonly ICsvData        data;
            private readonly CsvDataReader   reader;
            private readonly Type            rowType;
            private readonly FieldInfo       keyField;
            private readonly List<FieldInfo> normalFields;
            private readonly List<FieldInfo> nestedFields;

            private readonly Dictionary<FieldInfo, CsvParser> nestedParsers = new Dictionary<FieldInfo, CsvParser>();

            public CsvParser(ICsvData data, CsvDataReader reader)
            {
                this.data                              = data;
                this.reader                            = reader;
                this.rowType                           = data.RowType;
                this.keyField                          = this.rowType.GetCsvKeyField();
                (this.normalFields, this.nestedFields) = this.rowType.GetCsvFields().Split(field => !typeof(ICsvData).IsAssignableFrom(field.FieldType));
            }

            public void Parse()
            {
                var keyValue = (object)null;
                var row      = Activator.CreateInstance(this.rowType);

                foreach (var field in this.normalFields)
                {
                    try
                    {
                        var ordinal = this.reader.GetOrdinal(field.GetCsvFieldName());
                        var str     = this.reader.GetString(ordinal);
                        if (str.IsNullOrWhitespace()) continue;
                        var value = ConverterManager.Instance.ConvertFromString(str, field.FieldType);
                        field.SetValue(row, value);
                        if (this.keyField == field) keyValue = value;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException($"Field {field.GetCsvFieldName()}({field.Name}) not found in {this.rowType.Name}. If this is intentional, add [CsvIgnore] attribute to the field.");
                    }
                }

                if (keyValue is { })
                {
                    this.data.Add(keyValue, row);
                    this.nestedParsers.Clear();
                }

                foreach (var field in this.nestedFields)
                {
                    this.nestedParsers.GetOrAdd(field, () =>
                    {
                        var nestedData   = Activator.CreateInstance(field.FieldType);
                        var nestedParser = new CsvParser((ICsvData)nestedData, this.reader);
                        field.SetValue(row, nestedData);
                        return nestedParser;
                    }).Parse();
                }
            }
        }
    }
}