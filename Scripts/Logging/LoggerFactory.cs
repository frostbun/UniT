namespace UniT.Logging
{
    public class LoggerFactory : ILogger.IFactory
    {
        public ILogger Create(IHasLogger owner)
        {
            return new UnityLogger(owner.GetType().Name);
        }
    }
}