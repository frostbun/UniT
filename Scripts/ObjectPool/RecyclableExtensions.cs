namespace UniT.ObjectPool
{
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class RecyclableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Recycle<T>(this T component) where T : Component, IRecyclable
        {
            component.Manager.Recycle(component);
        }
    }
}