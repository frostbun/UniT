namespace UniT.Logging
{
    using UnityEngine.Scripting;

    public sealed class LoggerFactory : ILogger.IFactory
    {
        [Preserve]
        public LoggerFactory()
        {
        }

        public ILogger Create(IHasLogger owner)
        {
            return new UnityLogger(owner.GetType().Name);
        }
    }
}