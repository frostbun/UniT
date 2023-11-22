namespace UniT.Logging
{
    using System;

    public interface ILogger
    {
        public static Func<object, ILogger> Default { get; set; } = owner => new UnityLogger(owner.GetType().Name);

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