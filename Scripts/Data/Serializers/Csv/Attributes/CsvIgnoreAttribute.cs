#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CsvIgnoreAttribute : Attribute
    {
    }

    internal static class CsvIgnoreAttributeExtensions
    {
        private static bool IsCsvIgnored(this FieldInfo field)
        {
            return field.GetCustomAttribute<CsvIgnoreAttribute>() is { }
                || field.ToPropertyInfo()?.GetCustomAttribute<CsvIgnoreAttribute>() is { };
        }

        public static IEnumerable<FieldInfo> GetCsvFields(this Type type)
        {
            return type.GetAllFields().Where(field => !field.IsCsvIgnored());
        }
    }
}