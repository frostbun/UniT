namespace UniT.ObjectPool
{
    using UnityEngine;

    public static class RecyclableExtensions
    {
        public static void Recycle<T>(this T component) where T : Component, IRecyclable
        {
            component.Manager.Recycle(component);
        }
    }
}