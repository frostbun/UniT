namespace UniT.Data.Csv.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly List<FieldInfo>                  csvFields;
        private readonly List<FieldInfo>                  normalFields;
        private readonly Dictionary<FieldInfo, CsvParser> nestedParsers;

        public CsvParser(ICsvData data, CsvDataReader reader)
        {
            this.data                           = data;
            this.reader                         = reader;
            this.rowType                        = data.GetRowType();
            this.keyField                       = this.rowType.GetCsvKeyField();
            (this.csvFields, this.normalFields) = this.rowType.GetAllFields().Where(field => !field.IsCsvIgnored()).Split(field => typeof(ICsvData).IsAssignableFrom(field.FieldType));
            this.nestedParsers                  = new();
        }

        public void Parse()
        {
            var keyFieldIsSet = false;
            var row           = Activator.CreateInstance(this.rowType);

            foreach (var field in this.normalFields)
            {
                var ordinal = this.reader.GetOrdinal(field.GetCsvFieldName());
                if (ordinal == -1) throw new InvalidOperationException($"Field {field.Name} - {field.GetCsvFieldName()} not found in csv");
                var str = this.reader.GetString(ordinal);
                if (str.IsNullOrWhitespace()) continue;
                var converter = ConverterManager.Instance.GetConverter(field.FieldType);
                var value     = converter.ConvertFromString(str, field.FieldType);
                field.SetValue(row, value);
                if (field == this.keyField) keyFieldIsSet = true;
            }

            if (keyFieldIsSet) this.nestedParsers.Clear();

            foreach (var field in this.csvFields)
            {
                this.nestedParsers.GetOrAdd(field, () =>
                {
                    var nestedData   = Activator.CreateInstance(field.FieldType);
                    var nestedParser = new CsvParser((ICsvData)nestedData, this.reader);
                    field.SetValue(row, nestedData);
                    return nestedParser;
                }).Parse();
            }

            if (keyFieldIsSet) this.data.Add(this.keyField.GetValue(row), row);
        }
    }
}