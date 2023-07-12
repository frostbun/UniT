namespace UniT.Logging
{
    using System;

    public interface ILogger
    {
        public static class Factory
        {
            public static Func<string, ILogger> Default { private get; set; } = name => new Logger(name);

            public static ILogger CreateDefault(string name) => Default(name);
        }

        public string Name { get; }

        public LogConfig Config { get; }

        public void Debug(string message);

        public void Info(string message);

        public void Warning(string message);

        public void Error(string message);

        public void Critical(string message);

        public Exception Exception(Exception exception);
    }
}