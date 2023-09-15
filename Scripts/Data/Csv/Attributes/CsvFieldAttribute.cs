namespace UniT.Data.Csv.Attributes
{
    using System;
    using System.Reflection;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CsvFieldAttribute : Attribute
    {
        public string Name { get; }

        public CsvFieldAttribute(string name)
        {
            this.Name = name;
        }
    }

    public static class CsvFieldAttributeExtensions
    {
        public static string GetCsvFieldName(this FieldInfo field)
        {
            return field.GetCustomAttribute<CsvFieldAttribute>()?.Name
                ?? field.ToPropertyInfo()?.GetCustomAttribute<CsvFieldAttribute>()?.Name
                ?? field.Name.ToPropertyName();
        }
    }
}