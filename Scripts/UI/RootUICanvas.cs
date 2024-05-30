#nullable enable
namespace UniT.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
    public sealed class RootUICanvas : MonoBehaviour
    {
        public Transform Hiddens  { get; private set; } = null!;
        public Transform Screens  { get; private set; } = null!;
        public Transform Popups   { get; private set; } = null!;
        public Transform Overlays { get; private set; } = null!;

        private void Awake()
        {
            this.Hiddens  = this.CreateChild(nameof(this.Hiddens));
            this.Screens  = this.CreateChild(nameof(this.Screens));
            this.Popups   = this.CreateChild(nameof(this.Popups));
            this.Overlays = this.CreateChild(nameof(this.Overlays));
            this.Hiddens.gameObject.SetActive(false);
            DontDestroyOnLoad(this);
        }

        private Transform CreateChild(string name)
        {
            var child = new GameObject
            {
                name      = name,
                transform = { parent = this.transform },
            }.AddComponent<RectTransform>();
            child.anchorMin     = Vector2.zero;
            child.anchorMax     = Vector2.one;
            child.sizeDelta     = Vector2.zero;
            child.localPosition = Vector3.zero;
            return child;
        }
    }
}