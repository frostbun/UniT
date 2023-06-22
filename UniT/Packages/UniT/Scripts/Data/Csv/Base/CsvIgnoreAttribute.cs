namespace UniT.Data.Csv.Base
{
    using System;
    using System.Reflection;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CsvIgnoreAttribute : Attribute
    {
    }

    public static class CsvIgnoreAttributeExtensions
    {
        public static bool IsCsvIgnored(this FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttribute<CsvIgnoreAttribute>() is not null
                   || fieldInfo.ToPropertyInfo()?.GetCustomAttribute<CsvIgnoreAttribute>() is not null;
        }
    }
}