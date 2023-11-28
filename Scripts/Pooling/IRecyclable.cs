namespace UniT.Pooling
{
    using UnityEngine;

    public interface IRecyclable
    {
        public IObjectPoolManager Manager { get; set; }

        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();
    }

    public static class RecyclableExtensions
    {
        public static void Recycle<T>(this T component) where T : Component, IRecyclable
        {
            component.Manager.Recycle(component);
        }
    }
}