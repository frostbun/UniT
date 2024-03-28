namespace UniT.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
    public sealed class RootUICanvas : MonoBehaviour
    {
        [field: SerializeField] public RectTransform HiddenActivities   { get; private set; }
        [field: SerializeField] public RectTransform StackingActivities { get; private set; }
        [field: SerializeField] public RectTransform FloatingActivities { get; private set; }
        [field: SerializeField] public RectTransform DockedActivities   { get; private set; }
    }
}