namespace UniT.Data
{
    using System;
    using System.Reflection;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CsvColumnAttribute : Attribute
    {
        public string Name         { get; }
        public bool   IgnorePrefix { get; }

        public CsvColumnAttribute(string name, bool ignorePrefix = false)
        {
            this.Name         = name;
            this.IgnorePrefix = ignorePrefix;
        }
    }

    internal static class CsvColumnAttributeExtensions
    {
        public static string GetCsvColumn(this FieldInfo field, string prefix)
        {
            return (field.GetCustomAttribute<CsvColumnAttribute>() ?? field.ToPropertyInfo()?.GetCustomAttribute<CsvColumnAttribute>()) is { } attr
                ? attr.IgnorePrefix
                    ? attr.Name
                    : prefix + attr.Name
                : prefix + field.Name.ToPropertyName();
        }
    }
}