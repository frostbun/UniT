namespace UniT.Data
{
    using System;
    using System.Reflection;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CsvFieldAttribute : Attribute
    {
        public string Name         { get; }
        public bool   IgnorePrefix { get; }

        public CsvFieldAttribute(string name, bool ignorePrefix = false)
        {
            this.Name         = name;
            this.IgnorePrefix = ignorePrefix;
        }
    }

    internal static class CsvFieldAttributeExtensions
    {
        public static string GetCsvFieldName(this FieldInfo field, string prefix)
        {
            return (field.GetCustomAttribute<CsvFieldAttribute>() ?? field.ToPropertyInfo()?.GetCustomAttribute<CsvFieldAttribute>()) is { } attr
                ? attr.IgnorePrefix
                    ? attr.Name
                    : prefix + attr.Name
                : prefix + field.Name.ToPropertyName();
        }
    }
}