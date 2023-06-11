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
        private readonly ICsvData                      data;
        private readonly CsvDataReader                 reader;
        private readonly Type                          rowType;
        private readonly MemberInfo[]                  members;
        private readonly Dictionary<string, CsvParser> nestedParsers;

        public CsvParser(ICsvData data, CsvDataReader reader)
        {
            this.data          = data;
            this.reader        = reader;
            this.rowType       = data.GetRowType();
            this.members       = this.rowType.GetAllFieldsOrProperties();
            this.nestedParsers = new();
        }

        public void Parse()
        {
            var row = Activator.CreateInstance(this.rowType);
            foreach (var member in this.members)
            {
                var type = member switch
                {
                    FieldInfo field       => field.FieldType,
                    PropertyInfo property => property.PropertyType,
                    _                     => null,
                };
                if (typeof(ICsvData).IsAssignableFrom(type))
                {
                    this.nestedParsers.GetOrAdd(member.Name, () =>
                    {
                        var nestedData   = Activator.CreateInstance(type);
                        var nestedParser = new CsvParser((ICsvData)nestedData, this.reader);
                        switch (member)
                        {
                            case FieldInfo field:
                                field.SetValue(row, nestedData);
                                return nestedParser;
                            case PropertyInfo property:
                                property.SetValue(row, nestedData);
                                return nestedParser;
                            default:
                                return nestedParser;
                        }
                    }).Parse();
                    continue;
                }

                var ordinal   = this.reader.GetOrdinal(member.Name);
                var str       = this.reader.GetString(ordinal);
                var converter = ConverterManager.Instance.GetConverter(type);
                var value     = converter.ConvertFromString(str, type);
                switch (member)
                {
                    case FieldInfo field:
                        field.SetValue(row, value);
                        break;
                    case PropertyInfo property:
                        property.SetValue(row, value);
                        break;
                }
            }

            if (this.data.Add(row)) this.nestedParsers.Clear();
        }
    }
}