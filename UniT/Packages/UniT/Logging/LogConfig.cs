namespace UniT.Logging
{
    using System;

    public class LogConfig
    {
        public readonly LogLevel logLevel;
        
        public LogConfig(LogLevel logLevel = LogLevel.All)
        {
            this.logLevel = logLevel;
        }
    }

    [Flags]
    public enum LogLevel
    {
        Log       = 1 << 0,
        Warning   = 1 << 1,
        Error     = 1 << 2,
        Exception = 1 << 3,
        All       = -1,
    }
}