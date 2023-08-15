namespace UniT.Logging
{
    using System;

    public abstract class BaseLogger : ILogger
    {
        public string Name { get; }

        public LogConfig Config { get; }

        protected BaseLogger(string name, LogConfig config = null)
        {
            this.Name   = name;
            this.Config = config ?? new();
        }

        void ILogger.Debug(string message)
        {
            if (this.Config.Level > LogLevel.Debug) return;
            this.Debug($"[Debug][{this.Name}] {message}");
        }

        void ILogger.Info(string message)
        {
            if (this.Config.Level > LogLevel.Info) return;
            this.Info($"[Info][{this.Name}] {message}");
        }

        void ILogger.Warning(string message)
        {
            if (this.Config.Level > LogLevel.Warning) return;
            this.Warning($"[Warning][{this.Name}] {message}");
        }

        void ILogger.Error(string message)
        {
            if (this.Config.Level > LogLevel.Error) return;
            this.Error($"[Error][{this.Name}] {message}");
        }

        void ILogger.Critical(string message)
        {
            if (this.Config.Level > LogLevel.Critical) return;
            this.Critical($"[Critical][{this.Name}] {message}");
        }

        void ILogger.Exception(Exception exception)
        {
            if (this.Config.Level > LogLevel.Exception) return;
            this.Exception($"[Exception][{this.Name}] {exception.Message}", exception);
        }

        protected abstract void Debug(string message);

        protected abstract void Info(string message);

        protected abstract void Warning(string message);

        protected abstract void Error(string message);

        protected abstract void Critical(string message);

        protected abstract void Exception(string message, Exception exception);
    }
}