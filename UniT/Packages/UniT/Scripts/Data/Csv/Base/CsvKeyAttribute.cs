namespace UniT.Data.Csv.Base
{
    using System;
    using System.Reflection;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Class)]
    public class CsvKeyAttribute : Attribute
    {
        public readonly string key;

        public CsvKeyAttribute(string key)
        {
            this.key = key;
        }
    }

    public static class CsvKeyAttributeExtensions
    {
        public static FieldInfo GetCsvKeyField(this Type type)
        {
            var csvKey = type.GetCustomAttribute<CsvKeyAttribute>()?.key;
            return csvKey is null
                ? type.GetAllFields()[0]
                : type.GetField(csvKey)
                  ?? type.GetField($"<{csvKey}>k__BackingField")
                  ?? throw new InvalidOperationException($"Cannot find csv key {csvKey} in {type.Name}");
        }
    }
}