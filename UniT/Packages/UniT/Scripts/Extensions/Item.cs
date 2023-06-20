namespace UniT.Extensions
{
    public static class Item
    {
        public static T S<T>(T item) => item;

        public static bool IsTrue(bool item) => item;

        public static bool IsFalse(bool item) => !item;
    }
}