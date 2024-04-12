namespace UniT.Logging
{
    using UniT.Factories;
    using UnityEngine.Scripting;

    public sealed class LoggerFactory : ILogger.IFactory
    {
        [Preserve]
        public LoggerFactory()
        {
        }

        ILogger IFactory<IHasLogger, ILogger>.Create(IHasLogger owner)
        {
            return new UnityLogger(owner.GetType().Name);
        }
    }
}