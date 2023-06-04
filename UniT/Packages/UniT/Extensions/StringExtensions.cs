namespace UniT.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string Wrap(this string str, string wrapper)
        {
            return str.Wrap(wrapper, wrapper);
        }

        public static string Wrap(this string str, string prefix, string suffix)
        {
            return prefix + str + suffix;
        }
    }
}