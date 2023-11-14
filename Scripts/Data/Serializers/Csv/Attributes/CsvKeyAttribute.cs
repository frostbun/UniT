namespace UniT.Data.Serializers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CsvKeyAttribute : Attribute
    {
    }

    public static class CsvKeyAttributeExtensions
    {
        private static bool IsCsvKeyField(this FieldInfo field)
        {
            return field.GetCustomAttribute<CsvKeyAttribute>() is not null
                || field.ToPropertyInfo()?.GetCustomAttribute<CsvKeyAttribute>() is not null;
        }

        public static FieldInfo GetCsvKeyField(this Type type)
        {
            return type.GetAllFields().FirstOrDefault(field => field.IsCsvKeyField())
                ?? type.GetCsvFields().FirstOrDefault()
                ?? throw new InvalidOperationException($"Cannot find csv key field in {type.Name}");
        }
    }
}