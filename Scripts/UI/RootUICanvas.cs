namespace UniT.UI
{
    using UniT.Extensions;
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
            this.DontDestroyOnLoad();
        }

        private Transform CreateChild(string name)
        {
            return new GameObject
            {
                name      = name,
                transform = { parent = this.transform },
            }.transform;
        }
    }
}