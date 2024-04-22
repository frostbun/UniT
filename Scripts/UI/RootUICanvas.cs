namespace UniT.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
    public sealed class RootUICanvas : MonoBehaviour
    {
        public Transform HiddenActivities   { get; private set; }
        public Transform StackingActivities { get; private set; }
        public Transform FloatingActivities { get; private set; }
        public Transform DockedActivities   { get; private set; }

        private void Awake()
        {
            this.HiddenActivities   = this.CreateChild(nameof(this.HiddenActivities));
            this.StackingActivities = this.CreateChild(nameof(this.StackingActivities));
            this.FloatingActivities = this.CreateChild(nameof(this.FloatingActivities));
            this.DockedActivities   = this.CreateChild(nameof(this.DockedActivities));
            this.HiddenActivities.gameObject.SetActive(false);
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