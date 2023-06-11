namespace UniT.Data.Csv.Base
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Sylvan.Data.Csv;
    using UniT.Data.Converters.Base;
    using UniT.Extensions;

    public class CsvParser
    {
        private readonly ICsvData                         data;
        private readonly CsvDataReader                    reader;
        private readonly Type                             rowType;
        private readonly FieldInfo                        keyField;
        private readonly FieldInfo[]                      fields;
        private readonly Dictionary<FieldInfo, CsvParser> nestedParsers;

        public CsvParser(ICsvData data, CsvDataReader reader)
        {
            this.data          = data;
            this.reader        = reader;
            this.rowType       = data.GetRowType();
            this.keyField      = this.rowType.GetCsvKeyField();
            this.fields        = this.rowType.GetAllFields();
            this.nestedParsers = new();
        }

        public void Parse()
        {
            var row = Activator.CreateInstance(this.rowType);
            foreach (var field in this.fields)
            {
                if (typeof(ICsvData).IsAssignableFrom(field.FieldType))
                {
                    this.nestedParsers.GetOrAdd(field, () =>
                    {
                        var nestedData   = Activator.CreateInstance(field.FieldType);
                        var nestedParser = new CsvParser((ICsvData)nestedData, this.reader);
                        field.SetValue(row, nestedData);
                        return nestedParser;
                    }).Parse();
                    continue;
                }

                var ordinal   = this.reader.GetOrdinal(field.Name.ToPropertyName());
                var str       = this.reader.GetString(ordinal);
                var converter = ConverterManager.Instance.GetConverter(field.FieldType);
                var value     = converter.ConvertFromString(str, field.FieldType);
                field.SetValue(row, value);
            }

            var keyValue = this.keyField.GetValue(row);
            if (keyValue is null) return;
            this.data.Add(keyValue, row);
            this.nestedParsers.Clear();
        }
    }
}