namespace UniT.Logging
{
    using UniT.Factories;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class LoggerFactory : ILogger.IFactory
    {
        ILogger IFactory<IHasLogger, ILogger>.Create(IHasLogger owner)
        {
            return new UnityLogger(owner.GetType().Name);
        }
    }
}