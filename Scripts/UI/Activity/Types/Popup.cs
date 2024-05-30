#nullable enable
namespace UniT.UI.Activity
{
    public abstract class Popup : Activity, IPopup
    {
    }

    public abstract class Popup<TParams> : Activity<TParams>, IPopup
    {
    }
}