namespace UniT.Logging
{
    using System;
    using UniT.Factories;

    public interface ILogger
    {
        public interface IFactory : IFactory<IHasLogger, ILogger>
        {
        }

        public string Name { get; }

        public LogConfig Config { get; }

        public void Debug(string message);

        public void Info(string message);

        public void Warning(string message);

        public void Error(string message);

        public void Critical(string message);

        public void Exception(Exception exception);
    }
}