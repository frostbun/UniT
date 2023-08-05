namespace UniT.Data.Csv.Reader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Sylvan.Data.Csv;
    using UniT.Data.Converters;
    using UniT.Data.Csv.Attributes;
    using UniT.Extensions;

    public class CsvParser
    {
        private readonly ICsvData                         _data;
        private readonly CsvDataReader                    _reader;
        private readonly Type                             _rowType;
        private readonly FieldInfo                        _keyField;
        private readonly List<FieldInfo>                  _csvFields;
        private readonly List<FieldInfo>                  _normalFields;
        private readonly Dictionary<FieldInfo, CsvParser> _nestedParsers;

        public CsvParser(ICsvData data, CsvDataReader reader)
        {
            this._data                            = data;
            this._reader                          = reader;
            this._rowType                         = data.RowType;
            this._keyField                        = this._rowType.GetCsvKeyField();
            (this._csvFields, this._normalFields) = this._rowType.GetAllFields().Where(field => !field.IsCsvIgnored()).Split(field => typeof(ICsvData).IsAssignableFrom(field.FieldType));
            this._nestedParsers                   = new();
        }

        public void Parse()
        {
            var keyFieldIsSet = false;
            var row           = Activator.CreateInstance(this._rowType);

            foreach (var field in this._normalFields)
            {
                try
                {
                    var ordinal = this._reader.GetOrdinal(field.GetCsvFieldName());
                    var str     = this._reader.GetString(ordinal);
                    if (str.IsNullOrWhitespace()) continue;
                    var value = ConverterManager.Instance.ConvertFromString(str, field.FieldType);
                    field.SetValue(row, value);
                    if (field == this._keyField) keyFieldIsSet = true;
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException($"Field {field.Name} - {field.GetCsvFieldName()} not found in {this._data.GetType().Name}. If this is intentional, add the [CsvIgnore] attribute to the field.");
                }
            }

            if (keyFieldIsSet) this._nestedParsers.Clear();

            foreach (var field in this._csvFields)
            {
                this._nestedParsers.GetOrAdd(field, () =>
                {
                    var nestedData   = Activator.CreateInstance(field.FieldType);
                    var nestedParser = new CsvParser((ICsvData)nestedData, this._reader);
                    field.SetValue(row, nestedData);
                    return nestedParser;
                }).Parse();
            }

            if (keyFieldIsSet) this._data.Add(this._keyField.GetValue(row), row);
        }
    }
}