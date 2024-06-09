#if UNIT_CSV
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CsvRowAttribute : Attribute
    {
        public string  Prefix { get; }
        public string? Key    { get; }

        public CsvRowAttribute(string prefix, string? key = null)
        {
            this.Prefix = prefix;
            this.Key    = key;
        }
    }

    public static class CsvRowAttributeExtensions
    {
        public static (string Prefix, string? Key) GetCsvRow(this Type type)
        {
            return type.GetCustomAttribute<CsvRowAttribute>() is { } attr
                ? (attr.Prefix, attr.Key)
                : ("", null);
        }
    }
}
#endif