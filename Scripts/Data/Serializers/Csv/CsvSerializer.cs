namespace UniT.Data.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
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
            var reader = new CsvReader(rawData);
            var parser = new CsvParser((ICsvData)data, reader);
            while (reader.Read()) parser.Parse();
        }

        string ISerializer.Serialize(IData data)
        {
            throw new NotImplementedException();
        }

        private sealed class CsvReader
        {
            private readonly ReadOnlyCollection<ReadOnlyCollection<string>> data;
            private readonly Dictionary<string, int>                        columnNameToIndex;

            public CsvReader(string rawData)
            {
                // https://stackoverflow.com/questions/11456850/split-a-string-by-commas-but-ignore-commas-within-double-quotes-using-javascript
                this.data = rawData.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                    .Select(row => Regex.Split(row, ",(?=(?:(?:[^\"]*\"){2})*[^\"]*$)")
                        .Select(cell => cell.Trim().Trim('"').Replace("\"\"", "\""))
                        .ToReadOnlyCollection()
                    ).ToReadOnlyCollection();
                if (this.data.Count == 0) throw new InvalidOperationException("Empty CSV data");
                this.columnNameToIndex = this.data[0].Select((columnName, index) => (columnName, index)).ToDictionary();
            }

            private int currentRow = 0;

            public bool Read()
            {
                return ++this.currentRow < this.data.Count;
            }

            public bool ContainsColumn(string columnName)
            {
                return this.columnNameToIndex.ContainsKey(columnName);
            }

            public string GetCell(string columnName)
            {
                if (this.currentRow >= this.data.Count) throw new InvalidOperationException("End of CSV data");
                if (!this.columnNameToIndex.TryGetValue(columnName, out var index)) throw new InvalidOperationException($"Column {columnName} not found");
                return this.data[this.currentRow][index];
            }
        }

        private sealed class CsvParser
        {
            private readonly ICsvData        data;
            private readonly CsvReader       reader;
            private readonly Type            rowType;
            private readonly FieldInfo       keyField;
            private readonly List<FieldInfo> normalFields;
            private readonly List<FieldInfo> nestedFields;

            private readonly Dictionary<FieldInfo, CsvParser> nestedParsers = new Dictionary<FieldInfo, CsvParser>();

            public CsvParser(ICsvData data, CsvReader reader)
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
                    var columnName = field.GetCsvFieldName();
                    if (!this.reader.ContainsColumn(columnName)) throw new InvalidOperationException($"Field {columnName} not found in {this.rowType.Name}. If this is intentional, add [CsvIgnore] attribute to the field.");
                    var str = this.reader.GetCell(columnName);
                    if (str.IsNullOrWhitespace()) continue;
                    var value = ConverterManager.Instance.ConvertFromString(str, field.FieldType);
                    field.SetValue(row, value);
                    if (this.keyField == field) keyValue = value;
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