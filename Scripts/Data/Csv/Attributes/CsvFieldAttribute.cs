namespace UniT.Data.Csv.Attributes
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CsvFieldAttribute : Attribute
    {
        public string Name { get; }

        public CsvFieldAttribute(string name)
        {
            this.Name = name;
        }
    }

    public static class CsvFieldAttributeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCsvFieldName(this FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttribute<CsvFieldAttribute>()?.Name
                   ?? fieldInfo.ToPropertyInfo()?.GetCustomAttribute<CsvFieldAttribute>()?.Name
                   ?? fieldInfo.Name.ToPropertyName();
        }
    }
}