namespace UniT.Data.Csv.Base
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Sylvan.Data.Csv;
    using UniT.Data.Csv.Converters.Base;
    using UniT.Extensions;

    public class CsvParser
    {
        private readonly ICsvData                      data;
        private readonly MemberInfo[]                  members;
        private readonly Dictionary<string, CsvParser> nestedParsers;

        public CsvParser(ICsvData data)
        {
            this.data          = data;
            this.members       = data.GetRowType().GetAllFieldsOrProperties();
            this.nestedParsers = new();
        }

        public void ParseRow(CsvDataReader reader)
        {
            var row = Activator.CreateInstance(this.data.GetRowType());
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
                        var nestedData   = (ICsvData)Activator.CreateInstance(type);
                        var nestedParser = new CsvParser(nestedData);
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
                    }).ParseRow(reader);
                    continue;
                }

                var ordinal   = reader.GetOrdinal(member.Name);
                var str       = reader.GetString(ordinal);
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

            if (this.data.AddRow(row)) this.nestedParsers.Clear();
        }
    }
}