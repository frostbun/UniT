namespace UniT.Data.Serializers
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

    public static class CsvIgnoreAttributeExtensions
    {
        private static bool IsCsvIgnored(this FieldInfo field)
        {
            return field.GetCustomAttribute<CsvIgnoreAttribute>() is not null
                || field.ToPropertyInfo()?.GetCustomAttribute<CsvIgnoreAttribute>() is not null;
        }

        public static IEnumerable<FieldInfo> GetCsvFields(this Type type)
        {
            return type.GetAllFields().Where(field => !field.IsCsvIgnored());
        }
    }
}