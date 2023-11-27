namespace UniT.UI
{
    using UnityEngine;

    public interface IView
    {
        public IUIManager Manager { get; set; }

        public void OnInitialize();

        public GameObject gameObject { get; }

        public Transform transform { get; }

        public T[] GetComponentsInChildren<T>();
    }
}