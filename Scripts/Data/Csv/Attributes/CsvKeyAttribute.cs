namespace UniT.Data.Csv.Attributes
{
    using System;
    using System.Linq;
    using System.Reflection;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Class)]
    public class CsvKeyAttribute : Attribute
    {
        public string Key { get; }

        public CsvKeyAttribute(string key)
        {
            this.Key = key;
        }
    }

    public static class CsvKeyAttributeExtensions
    {
        public static FieldInfo GetCsvKeyField(this Type type)
        {
            var csvKey = type.GetCustomAttribute<CsvKeyAttribute>()?.Key;
            return csvKey is null
                ? type.GetAllFields().FirstOrDefault(field => !field.IsCsvIgnored())
                    ?? throw new InvalidOperationException($"Cannot find any csv field in {type.Name}")
                : type.GetField(csvKey)
                  ?? type.GetField(csvKey.ToBackingFieldName())
                  ?? throw new InvalidOperationException($"Cannot find csv key field {csvKey} in {type.Name}");
        }
    }
}