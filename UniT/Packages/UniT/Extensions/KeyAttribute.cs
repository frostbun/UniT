namespace UniT.Extensions
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class KeyAttribute : Attribute
    {
        public readonly string key;

        public KeyAttribute(string key)
        {
            this.key = key;
        }
    }
}