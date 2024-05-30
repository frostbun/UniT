#nullable enable
namespace UniT.UI.Activity
{
    public abstract class Overlay : Activity, IOverlay
    {
    }

    public abstract class Overlay<TParams> : Activity<TParams>, IOverlay
    {
    }
}