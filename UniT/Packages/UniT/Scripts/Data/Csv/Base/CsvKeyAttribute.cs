namespace UniT.Data.Csv.Base
{
    using System;
    using System.Reflection;
    using UniT.Extensions;

    [AttributeUsage(AttributeTargets.Class)]
    public class CsvKeyAttribute : Attribute
    {
        public readonly string key;

        public CsvKeyAttribute(string key)
        {
            this.key = key;
        }
    }

    public static class CsvKeyAttributeExtensions
    {
        public static string GetCsvKeyAttribute(this Type type)
        {
            return type.GetCustomAttribute<CsvKeyAttribute>()?.key ?? type.GetAllFieldsOrProperties()[0].Name;
        }
    }
}