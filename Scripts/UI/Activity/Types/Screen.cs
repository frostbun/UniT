#nullable enable
namespace UniT.UI.Activity
{
    public abstract class Screen : Activity, IScreen
    {
    }

    public abstract class Screen<TParams> : Activity<TParams>, IScreen
    {
    }
}