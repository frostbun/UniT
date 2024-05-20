namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using UniT.Extensions;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    using System.Threading.Tasks;
    #endif

    public sealed class CsvSerializer : IStringSerializer
    {
        [Preserve]
        public CsvSerializer()
        {
        }

        bool ISerializer.CanSerialize(Type type) => typeof(ICsvData).IsAssignableFrom(type);

        void IStringSerializer.Populate(IData data, string rawData) => Populate(data, rawData);

        string IStringSerializer.Serialize(IData data) => Serialize(data);

        #if UNIT_UNITASK
        UniTask IStringSerializer.PopulateAsync(IData data, string rawData) => UniTask.RunOnThreadPool(() => Populate(data, rawData));

        UniTask<string> IStringSerializer.SerializeAsync(IData data) => UniTask.RunOnThreadPool(() => Serialize(data));
        #else
        IEnumerator IStringSerializer.PopulateAsync(IData data, string rawData, Action callback) => Task.Run(() => Populate(data, rawData)).ToCoroutine(callback);

        IEnumerator IStringSerializer.SerializeAsync(IData data, Action<string> callback) => Task.Run(() => Serialize(data)).ToCoroutine(callback);
        #endif

        private static void Populate(IData data, string rawData)
        {
            var reader = new CsvReader(rawData);
            var parser = new CsvParser((ICsvData)data, reader);
            while (reader.Read()) parser.Parse();
        }

        private static string Serialize(IData data)
        {
            throw new NotImplementedException();
        }

        private sealed class CsvReader
        {
            private readonly string[][]              data;
            private readonly Dictionary<string, int> columnNameToIndex;

            public CsvReader(string rawData)
            {
                // https://stackoverflow.com/questions/11456850/split-a-string-by-commas-but-ignore-commas-within-double-quotes-using-javascript
                this.data = rawData.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                    .Select(row => Regex.Split(row, ",(?=(?:(?:[^\"]*\"){2})*[^\"]*$)")
                        .Select(cell => cell.Trim().Trim('"').Replace("\"\"", "\""))
                        .ToArray()
                    ).ToArray();
                if (this.data.Length == 0) throw new InvalidOperationException("Empty CSV data");
                this.columnNameToIndex = this.data[0].Select((columnName, index) => (columnName, index)).ToDictionary();
            }

            private int currentRow = 0;

            public bool Read()
            {
                return ++this.currentRow < this.data.Length;
            }

            public bool ContainsColumn(string columnName)
            {
                return this.columnNameToIndex.ContainsKey(columnName);
            }

            public string GetCell(string columnName)
            {
                return this.data[this.currentRow][this.columnNameToIndex[columnName]];
            }
        }

        private sealed class CsvParser
        {
            private readonly ICsvData        data;
            private readonly CsvReader       reader;
            private readonly FieldInfo       keyField;
            private readonly List<FieldInfo> normalFields;
            private readonly List<FieldInfo> nestedFields;

            private readonly Dictionary<FieldInfo, CsvParser> nestedParsers = new Dictionary<FieldInfo, CsvParser>();

            public CsvParser(ICsvData data, CsvReader reader)
            {
                this.data   = data;
                this.reader = reader;
                var csvFields = data.RowType.GetCsvFields().ToArray();
                this.keyField                          = data.Key.IsNullOrWhitespace() ? csvFields.First() : csvFields.First(field => field.Name == data.Key);
                (this.normalFields, this.nestedFields) = csvFields.Split(field => !typeof(ICsvData).IsAssignableFrom(field.FieldType));
                this.normalFields.ForEach(field =>
                {
                    var columnName = field.GetCsvFieldName(this.data.Prefix);
                    if (!this.reader.ContainsColumn(columnName)) throw new InvalidOperationException($"Field {columnName} - {this.data.RowType.Name} not found. If this is intentional, add [CsvIgnore] attribute to the field.");
                });
            }

            public void Parse()
            {
                var keyValue = default(object);
                var row      = Activator.CreateInstance(this.data.RowType);

                this.normalFields.ForEach(field =>
                {
                    var str = this.reader.GetCell(field.GetCsvFieldName(this.data.Prefix));
                    if (str.IsNullOrWhitespace()) return;
                    var value = ConverterManager.ConvertFromString(str, field.FieldType);
                    field.SetValue(row, value);
                    if (field == this.keyField) keyValue = value;
                });

                if (keyValue is { })
                {
                    this.data.Add(keyValue, row);
                    this.nestedParsers.Clear();
                }

                this.nestedFields.ForEach(field =>
                    this.nestedParsers.GetOrAdd(field, () =>
                    {
                        var nestedData   = Activator.CreateInstance(field.FieldType);
                        var nestedParser = new CsvParser((ICsvData)nestedData, this.reader);
                        field.SetValue(row, nestedData);
                        return nestedParser;
                    }).Parse()
                );
            }
        }
    }
}