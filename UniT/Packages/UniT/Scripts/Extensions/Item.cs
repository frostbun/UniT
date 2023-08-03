namespace UniT.Extensions
{
    using System.Runtime.CompilerServices;

    public static class Item
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T S<T>(T item) => item;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTrue(bool item) => item;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFalse(bool item) => !item;
    }
}