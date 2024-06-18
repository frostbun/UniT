#if UNIT_CSV
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using CsvHelper;
    using CsvHelper.Configuration;
    using UniT.Data.Conversion;
    using UniT.Extensions;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #endif

    public sealed class CsvSerializer : IStringSerializer
    {
        private readonly IConverterManager converterManager;
        private readonly CsvConfiguration  configuration;

        [Preserve]
        public CsvSerializer(IConverterManager converterManager, CsvConfiguration? configuration = null)
        {
            this.converterManager = converterManager;
            this.configuration = configuration
                ?? new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound     = null,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                };
        }

        bool ISerializer.CanSerialize(Type type) => typeof(ICsvData).IsAssignableFrom(type);

        void IStringSerializer.Populate(IData data, string rawData) => this.Populate(data, rawData);

        string IStringSerializer.Serialize(IData data) => this.Serialize(data);

        #if UNIT_UNITASK
        UniTask IStringSerializer.PopulateAsync(IData data, string rawData) => UniTask.RunOnThreadPool(() => this.Populate(data, rawData));

        UniTask<string> IStringSerializer.SerializeAsync(IData data) => UniTask.RunOnThreadPool(() => this.Serialize(data));
        #else
        IEnumerator IStringSerializer.PopulateAsync(IData data, string rawData, Action? callback) => CoroutineRunner.Run(() => this.Populate(data, rawData), callback);

        IEnumerator IStringSerializer.SerializeAsync(IData data, Action<string> callback) => CoroutineRunner.Run(() => this.Serialize(data), callback);
        #endif

        private void Populate(IData data, string rawData)
        {
            using var reader = new CsvReader(new StringReader(rawData), this.configuration);
            if (!reader.Read()) return;
            reader.ReadHeader();
            var populator = new Populator(this.converterManager, (ICsvData)data, reader);
            while (reader.Read()) populator.Populate();
        }

        private string Serialize(IData data)
        {
            using var stringWriter = new StringWriter();
            using var writer       = new CsvWriter(stringWriter, this.configuration);
            var       serializer   = new Serializer(this.converterManager, (ICsvData)data, writer);

            var hasValue = serializer.MoveNext();
            if (!hasValue) return string.Empty;

            foreach (var header in serializer.GetHeaders())
            {
                writer.WriteField(header);
            }
            writer.NextRecord();

            while (hasValue)
            {
                serializer.Serialize();
                writer.NextRecord();
                hasValue = serializer.MoveNext();
            }

            return stringWriter.ToString();
        }

        private sealed class Populator
        {
            private readonly IConverterManager                                                 converterManager;
            private readonly ICsvData                                                          data;
            private readonly CsvReader                                                         reader;
            private readonly FieldInfo                                                         keyField;
            private readonly IReadOnlyDictionary<FieldInfo, (int Index, IConverter Converter)> normalFields;
            private readonly IReadOnlyList<FieldInfo>                                          nestedFields;

            private readonly Dictionary<FieldInfo, Populator> nestedPopulators = new Dictionary<FieldInfo, Populator>();

            public Populator(IConverterManager converterManager, ICsvData data, CsvReader reader)
            {
                this.converterManager = converterManager;
                this.data             = data;
                this.reader           = reader;
                var rowType = data.RowType;
                var (prefix, key) = rowType.GetCsvRow();
                var csvFields = rowType.GetCsvFields().ToArray();
                this.keyField                    = key.IsNullOrWhitespace() ? csvFields.First() : csvFields.First(field => field.Name == key);
                var (normalFields, nestedFields) = csvFields.Split(field => !typeof(ICsvData).IsAssignableFrom(field.FieldType));
                this.normalFields = normalFields.ToDictionary(
                    field => field,
                    field =>
                    {
                        var column = field.GetCsvColumn(prefix);
                        var index  = reader.GetFieldIndex(column);
                        if (index < 0) throw new InvalidOperationException($"Column {column} not found in {rowType.Name}. If this is intentional, add [CsvIgnore] attribute to the field.");
                        return (index, this.converterManager.GetConverter(field.FieldType));
                    }
                );
                this.nestedFields = nestedFields;
            }

            public void Populate()
            {
                var keyValue = default(object);
                var row      = Activator.CreateInstance(this.data.RowType);

                foreach (var (field, (index, converter)) in this.normalFields)
                {
                    var str = this.reader[index];
                    if (str.IsNullOrWhitespace()) continue;
                    var value = converter.ConvertFromString(str, field.FieldType);
                    field.SetValue(row, value);
                    if (field == this.keyField) keyValue = value;
                }

                if (keyValue is { })
                {
                    this.data.Add(keyValue, row);
                    this.nestedPopulators.Clear();
                }

                foreach (var field in this.nestedFields)
                {
                    this.nestedPopulators.GetOrAdd(field, () =>
                    {
                        var nestedData      = Activator.CreateInstance(field.FieldType);
                        var nestedPopulator = new Populator(this.converterManager, (ICsvData)nestedData, this.reader);
                        field.SetValue(row, nestedData);
                        return nestedPopulator;
                    }).Populate();
                }
            }
        }

        private sealed class Serializer
        {
            private readonly IConverterManager                          converterManager;
            private readonly IEnumerator                                data;
            private readonly CsvWriter                                  writer;
            private readonly IReadOnlyList<string>                      headers;
            private readonly IReadOnlyDictionary<FieldInfo, IConverter> normalFields;
            private readonly IReadOnlyList<FieldInfo>                   nestedFields;

            private readonly Dictionary<FieldInfo, Serializer>            nestedSerializers = new Dictionary<FieldInfo, Serializer>();
            private readonly Dictionary<FieldInfo, IReadOnlyList<string>> nestedHeaders     = new Dictionary<FieldInfo, IReadOnlyList<string>>();

            public Serializer(IConverterManager converterManager, ICsvData data, CsvWriter writer)
            {
                this.converterManager = converterManager;
                // ReSharper disable once NotDisposedResource
                this.data   = data.GetEnumerator();
                this.writer = writer;
                var rowType = data.RowType;
                var (prefix, _)                  = rowType.GetCsvRow();
                var (normalFields, nestedFields) = rowType.GetCsvFields().Split(field => !typeof(ICsvData).IsAssignableFrom(field.FieldType));
                this.headers                     = normalFields.Select(field => field.GetCsvColumn(prefix)).ToArray();
                this.normalFields = normalFields.ToDictionary(
                    field => field,
                    field => this.converterManager.GetConverter(field.FieldType)
                );
                this.nestedFields = nestedFields;
            }

            public IEnumerable<string> GetHeaders()
            {
                return this.headers.Concat(this.nestedFields.SelectMany(field => this.nestedHeaders[field]));
            }

            private bool hasValue;

            public bool MoveNext()
            {
                if (
                    this.nestedSerializers.Count > 0
                    && this.nestedFields.Aggregate(false, (hasNestedValue, field) => hasNestedValue || this.nestedSerializers[field].MoveNext())
                ) return true;

                this.hasValue = this.data.MoveNext();
                if (!this.hasValue)
                {
                    (this.data as IDisposable)?.Dispose();
                    return false;
                }

                var row = this.data.Current;
                foreach (var field in this.nestedFields)
                {
                    var serializer = this.nestedSerializers[field] = new Serializer(this.converterManager, (ICsvData)field.GetValue(row), this.writer);
                    serializer.MoveNext();
                    this.nestedHeaders.TryAdd(field, () => serializer.GetHeaders().ToArray());
                }
                return true;
            }

            public void Serialize()
            {
                if (this.hasValue)
                {
                    var row = this.data.Current;
                    foreach (var (field, converter) in this.normalFields)
                    {
                        this.writer.WriteField(converter.ConvertToString(field.GetValue(row), field.FieldType));
                    }
                    this.hasValue = false;
                }
                else
                {
                    foreach (var _ in this.normalFields)
                    {
                        this.writer.WriteField(string.Empty);
                    }
                }

                foreach (var field in this.nestedFields)
                {
                    this.nestedSerializers[field].Serialize();
                }
            }
        }
    }
}
#endif