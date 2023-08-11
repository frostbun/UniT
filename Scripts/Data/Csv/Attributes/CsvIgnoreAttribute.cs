namespace UniT.Data.Csv.Attributes
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CsvIgnoreAttribute : Attribute
    {
    }

    public static class CsvIgnoreAttributeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCsvIgnored(this FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttribute<CsvIgnoreAttribute>() is not null
                   || fieldInfo.ToPropertyInfo()?.GetCustomAttribute<CsvIgnoreAttribute>() is not null;
        }
    }
}